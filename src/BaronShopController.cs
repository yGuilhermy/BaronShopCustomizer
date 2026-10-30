using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThunderRoad;

namespace BaronShopCustomizer
{
    public class BaronShopController : MonoBehaviour
    {
        private Shop activeShop;
        private Coroutine loopCoroutine;
        private bool isRefreshing = false;
        private bool refreshQueued;
        private int restockGeneration;
        private int pendingSpawns;
        private Coroutine restockCoroutine;

        public void Initialize()
        {
            Debug.Log("[BaronShopCustomizer] v1.1.1 ready");
            BaronShopModOptions.OnOptionsChanged += HandleOptionsChanged;
            OnLevelReady(null);
            BaronShopModOptions.OnFastRefreshRequested += HandleFastRefreshRequested;
        }

        private void OnDisable()
        {
            restockGeneration++;
            StopAllCoroutines();
            loopCoroutine = null;
            restockCoroutine = null;
            isRefreshing = false;
            refreshQueued = false;
            UnhookShop(activeShop);
            activeShop = null;
        }

        private void OnDestroy()
        {
            BaronShopModOptions.OnOptionsChanged -= HandleOptionsChanged;
            BaronShopModOptions.OnFastRefreshRequested -= HandleFastRefreshRequested;
            restockGeneration++;
            UnhookShop(activeShop);
        }

        public void OnLevelReady(LevelData levelData)
        {
            restockGeneration++;
            if (restockCoroutine != null) StopCoroutine(restockCoroutine);
            restockCoroutine = null;
            isRefreshing = false;
            refreshQueued = false;
            UnhookShop(activeShop);
            activeShop = null;
            if (loopCoroutine != null)
            {
                StopCoroutine(loopCoroutine);
            }
            loopCoroutine = StartCoroutine(ShopDetectionCoroutine());
        }

        private IEnumerator ShopDetectionCoroutine()
        {
            for (int i = 0; i < 30; i++)
            {
                if (activeShop == null)
                {
                    activeShop = Shop.local ?? UnityEngine.Object.FindObjectOfType<Shop>();
                }

                if (activeShop != null)
                {
                    HookShop(activeShop);
                    UpdateAllPriceTags();
                    yield break;
                }
                yield return new WaitForSecondsRealtime(1.0f);
            }
        }

        private void HookShop(Shop shop)
        {
            if (shop == null) return;
            shop.onItemEnterZone -= HandleItemEnterZone;
            shop.onItemEnterZone += HandleItemEnterZone;
            shop.onItemExitZone -= HandleItemExitZone;
            shop.onItemExitZone += HandleItemExitZone;
        }

        private void UnhookShop(Shop shop)
        {
            if (shop == null) return;
            shop.onItemEnterZone -= HandleItemEnterZone;
            shop.onItemExitZone -= HandleItemExitZone;
        }

        private void HandleItemEnterZone(Shop.Transaction transaction, Item item)
        {
            try
            {
                if (transaction == Shop.Transaction.Sell && item != null && item.data != null)
                {
                    float mult = BaronShopModOptions.generalSellMultiplier;
                    float basePrice = item.data.value;
                    item.SetValue((float)Math.Round(basePrice * mult));
                    if (activeShop != null)
                    {
                        activeShop.RefreshBuyingAndSellingValues();
                    }
                }
            }
            catch (Exception error)
            {
                Debug.LogWarning("[BaronShopCustomizer] " + error);
            }
        }

        private void HandleItemExitZone(Shop.Transaction transaction, Item item)
        {
            try
            {
                if (transaction == Shop.Transaction.Sell && item != null && item.data != null)
                {
                    item.SetValue(item.data.value);
                    if (activeShop != null)
                    {
                        activeShop.RefreshBuyingAndSellingValues();
                    }
                }
            }
            catch (Exception error)
            {
                Debug.LogWarning("[BaronShopCustomizer] " + error);
            }
        }

        private void HandleOptionsChanged()
        {
            if (!isActiveAndEnabled) return;
            UpdateAllPriceTags();
            if (activeShop != null)
            {
                foreach (var item in new List<Item>(activeShop.sellingItems ?? new List<Item>()))
                    if (item != null && item.data != null) item.SetValue((float)Math.Round(item.data.value * BaronShopModOptions.generalSellMultiplier));
                activeShop.RefreshBuyingAndSellingValues();
            }
        }

        private void HandleFastRefreshRequested()
        {
            if (!isActiveAndEnabled) return;
            if (activeShop == null)
            {
                activeShop = Shop.local ?? UnityEngine.Object.FindObjectOfType<Shop>();
            }

            if (activeShop == null) { Debug.LogWarning("[BaronShopCustomizer] No shop in this level"); return; }
            if (isRefreshing) { refreshQueued = true; return; }
            restockCoroutine = StartCoroutine(RestockSafely());
        }

        private void UpdateAllPriceTags()
        {
            try
            {
                if (activeShop != null && activeShop.displayStands != null)
                    foreach (var display in new List<ShopDisplay>(activeShop.displayStands))
                        if (display != null && display.displayItems != null)
                            foreach (var item in new List<Item>(display.displayItems))
                                if (item != null && item.owner == Item.Owner.Shopkeeper) ApplyPriceToItem(item);
                ShopPriceTag[] tags = UnityEngine.Object.FindObjectsOfType<ShopPriceTag>();
                if (tags == null || tags.Length == 0) return;

                for (int i = 0; i < tags.Length; i++)
                {
                    ShopPriceTag tag = tags[i];
                    if (tag == null) continue;

                    Item item = tag.GetComponentInParent<Item>();
                    if (item == null || item.data == null || item.owner != Item.Owner.Shopkeeper) continue;

                    float basePrice = item.data.value;
                    float multiplier = GetBuyMultiplierForItem(item.data);
                    float finalPrice = (float)Math.Round(basePrice * multiplier);

                    item.SetValue(finalPrice);
                    tag.Set(finalPrice, item.data.tier);
                }
            }
            catch (Exception error)
            {
                Debug.LogWarning("[BaronShopCustomizer] " + error);
            }
        }

        private void ApplyPriceToItem(Item item)
        {
            if (item == null || item.data == null) return;
            try
            {
                float basePrice = item.data.value;
                float multiplier = GetBuyMultiplierForItem(item.data);
                float finalPrice = (float)Math.Round(basePrice * multiplier);
                item.SetValue(finalPrice);
            }
            catch (Exception error)
            {
                Debug.LogWarning("[BaronShopCustomizer] " + error);
            }
        }

        private float GetBuyMultiplierForItem(ItemData itemData)
        {
            if (itemData == null) return 1.0f;

            if (BaronShopModOptions.priceMode == "General")
            {
                return BaronShopModOptions.generalBuyMultiplier;
            }

            if (IsWeapon(itemData))
            {
                return BaronShopModOptions.weaponBuyMultiplier;
            }

            if (IsArmor(itemData))
            {
                return BaronShopModOptions.armorBuyMultiplier;
            }

            if (IsClothes(itemData))
            {
                return BaronShopModOptions.clothesBuyMultiplier;
            }

            if (IsConsumable(itemData))
            {
                return BaronShopModOptions.consumableBuyMultiplier;
            }

            return 1.0f;
        }

        private IEnumerator RestockSafely()
        {
            isRefreshing = true;
            int generation = ++restockGeneration;
            pendingSpawns = 0;
            try
            {
                var routine = PerformFastRestockRoutine();
                while (true)
                {
                    bool next;
                    try { next = routine.MoveNext(); }
                    catch (Exception error) { Debug.LogError("[BaronShopCustomizer] Restock failed: " + error); break; }
                    if (!next) break;
                    yield return routine.Current;
                }
                float deadline = Time.unscaledTime + 20f;
                while (pendingSpawns > 0 && Time.unscaledTime < deadline) yield return null;
                if (pendingSpawns > 0)
                {
                    Debug.LogWarning("[BaronShopCustomizer] Spawn timeout: " + pendingSpawns);
                    restockGeneration++;
                }
                UpdateAllPriceTags();
                if (activeShop != null) activeShop.RefreshBuyingAndSellingValues();
                Debug.Log("[BaronShopCustomizer] Restock finished generation=" + generation);
            }
            finally
            {
                isRefreshing = false;
                restockCoroutine = null;
            }
            if (refreshQueued) { refreshQueued = false; HandleFastRefreshRequested(); }
        }

        private IEnumerator PerformFastRestockRoutine()
        {
            int generation = restockGeneration;
            Shop shop = activeShop;
            List<ItemData> allItems = Catalog.GetDataList<ItemData>();
            if (allItems == null || allItems.Count == 0)
            {
                Debug.LogWarning("[BaronShopCustomizer] Item catalog unavailable");
                yield break;
            }

            bool isGeneral = BaronShopModOptions.filterMode == "General";
            int weaponTier = isGeneral ? BaronShopModOptions.generalTierFilter : BaronShopModOptions.weaponTierFilter;
            string weaponCat = isGeneral ? "All" : BaronShopModOptions.weaponCategoryFilter;
            int apparelTier = isGeneral ? BaronShopModOptions.generalTierFilter : BaronShopModOptions.apparelTierFilter;
            string apparelType = isGeneral ? "All" : BaronShopModOptions.apparelTypeFilter;
            string consumableType = isGeneral ? "All" : BaronShopModOptions.consumableTypeFilter;

            List<ItemData> weaponPool = FilterWeapons(allItems, weaponTier, weaponCat);
            List<ItemData> apparelPool = FilterApparel(allItems, apparelTier, apparelType);
            List<ItemData> consumablePool = FilterConsumables(allItems, consumableType);
            if (isGeneral && BaronShopModOptions.generalTierFilter >= 0)
                consumablePool.RemoveAll(item => item.tier != BaronShopModOptions.generalTierFilter);
            if (weaponPool.Count == 0 || apparelPool.Count == 0 || consumablePool.Count == 0)
                Debug.LogWarning("[BaronShopCustomizer] Empty filter result: existing stock of that category will be preserved");

            ShuffleList(weaponPool);
            ShuffleList(apparelPool);
            ShuffleList(consumablePool);

            int weaponIndex = 0;
            int apparelIndex = 0;
            int consumableIndex = 0;

            List<ShopDisplay> displays = activeShop.displayStands;
            if (displays == null || displays.Count == 0)
            {
                displays = new List<ShopDisplay>(activeShop.GetComponentsInChildren<ShopDisplay>(true));
                foreach (var candidate in UnityEngine.Object.FindObjectsOfType<ShopDisplay>(true))
                    if ((candidate.shop == activeShop || candidate.shop == null) && !displays.Contains(candidate)) displays.Add(candidate);
            }

            Debug.Log("[BaronShopCustomizer] Restock displays=" + displays.Count + " weapons=" + weaponPool.Count + " apparel=" + apparelPool.Count + " consumables=" + consumablePool.Count);
            if (displays != null && displays.Count > 0)
            {
                for (int d = 0; d < displays.Count; d++)
                {
                    ShopDisplay display = displays[d];
                    if (display == null || !display.gameObject.activeInHierarchy) continue;

                    List<Holder> holders = GetDisplayHolders(display);
                    List<ItemShelf> shelves = GetDisplayShelves(display);
                    List<ItemBasket> baskets = GetDisplayBaskets(display);

                    bool isWeaponDisplay = display.displayType == ShopDisplay.DisplayType.Holders
                        || holders.Count > 0
                        || (!string.IsNullOrEmpty(display.displayCategory) && display.displayCategory.IndexOf("weapon", StringComparison.OrdinalIgnoreCase) >= 0);

                    bool isApparelDisplay = display.displayType == ShopDisplay.DisplayType.Shelf
                        || shelves.Count > 0
                        || (!string.IsNullOrEmpty(display.displayCategory) && (display.displayCategory.IndexOf("armor", StringComparison.OrdinalIgnoreCase) >= 0 || display.displayCategory.IndexOf("apparel", StringComparison.OrdinalIgnoreCase) >= 0 || display.displayCategory.IndexOf("cloth", StringComparison.OrdinalIgnoreCase) >= 0));

                    bool isConsumableDisplay = display.displayType == ShopDisplay.DisplayType.Basket
                        || baskets.Count > 0
                        || (!string.IsNullOrEmpty(display.displayCategory) && (display.displayCategory.IndexOf("consumable", StringComparison.OrdinalIgnoreCase) >= 0 || display.displayCategory.IndexOf("potion", StringComparison.OrdinalIgnoreCase) >= 0));

                    if (!(isWeaponDisplay && holders.Count > 0 && weaponPool.Count > 0)
                        && !(isApparelDisplay && shelves.Count > 0 && apparelPool.Count > 0)
                        && !(isConsumableDisplay && baskets.Count > 0 && consumablePool.Count > 0)) continue;

                    if (display.displayItems != null)
                    {
                        var previousItems = new List<Item>(display.displayItems);
                        for (int it = previousItems.Count - 1; it >= 0; it--)
                        {
                            Item oldItem = previousItems[it];
                            if (oldItem != null)
                            {
                                if (oldItem.IsHeld() || oldItem.owner != Item.Owner.Shopkeeper) continue;
                                if (isWeaponDisplay && oldItem.holder != null && !CanRefill(oldItem.holder, weaponPool)) continue;
                                if (oldItem.holder != null)
                                {
                                    oldItem.holder.UnSnap(oldItem, silent: true);
                                }
                                display.displayItems.Remove(oldItem);
                                oldItem.Despawn();
                            }
                        }
                        display.displayItems.RemoveAll(item => item == null);
                    }
                    else
                    {
                        display.displayItems = new List<Item>();
                    }

                    for (int h = 0; h < holders.Count; h++)
                    {
                        Holder holder = holders[h];
                        if (holder != null && holder.items != null && CanRefill(holder, weaponPool))
                        {
                            for (int it = holder.items.Count - 1; it >= 0; it--)
                            {
                                Item snappedItem = holder.items[it];
                                if (snappedItem != null)
                                {
                                    if (snappedItem.IsHeld() || snappedItem.owner != Item.Owner.Shopkeeper) continue;
                                    holder.UnSnap(snappedItem, silent: true);
                                    snappedItem.Despawn();
                                }
                            }
                        }
                    }

                    if (isWeaponDisplay && holders.Count > 0 && weaponPool.Count > 0)
                    {
                        foreach (var holder in holders)
                        {
                            if (holder == null || (holder.items != null && holder.items.Count > 0)) continue;
                            if (holder.data == null) holder.TryLoadFromID();
                            if (holder.data == null || holder.data.slots == null) continue;
                            var picked = StockSelection.FindCompatible(weaponPool,
                                data => !string.IsNullOrEmpty(data.slot) && holder.data.slots.Contains(data.slot), weaponIndex++);
                            if (picked == null) continue;
                            Holder targetHolder = holder;
                            ShopDisplay targetDisplay = display;
                                pendingSpawns++;
                                picked.SpawnAsync(spawnedItem =>
                                {
                                    try
                                    {
                                    if (generation != restockGeneration || shop == null || activeShop != shop || !isActiveAndEnabled || targetDisplay == null)
                                    { if (spawnedItem != null) spawnedItem.Despawn(); return; }
                                    if (targetHolder == null || (targetHolder.items != null && targetHolder.items.Count > 0)) { if (spawnedItem != null) spawnedItem.Despawn(); return; }
                                    if (spawnedItem != null)
                                    {
                                        spawnedItem.isUsed = false;
                                        spawnedItem.SetOwner(Item.Owner.Shopkeeper);
                                        ApplyPriceToItem(spawnedItem);
                                        if (activeShop != null)
                                        {
                                            activeShop.AddPriceTags(spawnedItem);
                                        }
                                        targetHolder.Snap(spawnedItem, silent: true);
                                        if (targetDisplay != null && targetDisplay.displayItems != null)
                                        {
                                            targetDisplay.displayItems.Add(spawnedItem);
                                        }

                                        if (spawnedItem.TryGetCustomReference<ItemSpawner>("SecondaryShopItem", out var custom))
                                        {
                                            UnityEngine.Events.UnityAction<Item> secondaryListener = null;
                                            secondaryListener = secondaryItem =>
                                            {
                                                if (custom != null) custom.onSpawnEvent.RemoveListener(secondaryListener);
                                                if (generation != restockGeneration || shop == null || activeShop != shop || !isActiveAndEnabled || targetDisplay == null)
                                                { if (secondaryItem != null && !secondaryItem.IsHeld() && secondaryItem.owner != Item.Owner.Player) secondaryItem.Despawn(); return; }
                                                if (secondaryItem != null)
                                                {
                                                    secondaryItem.isUsed = false;
                                                    secondaryItem.SetOwner(Item.Owner.Shopkeeper);
                                                    ApplyPriceToItem(secondaryItem);
                                                    if (activeShop != null)
                                                    {
                                                        activeShop.AddPriceTags(secondaryItem);
                                                    }
                                                    secondaryItem.SetVariable("SecondaryShopItem", true);
                                                    if (targetDisplay != null && targetDisplay.displayItems != null)
                                                    {
                                                        targetDisplay.displayItems.Add(secondaryItem);
                                                    }
                                                }
                                            };
                                            custom.onSpawnEvent.AddListener(secondaryListener);
                                            custom.Spawn();
                                        }
                                    }
                                
                                    }
                                    catch (Exception error)
                                    {
                                        Debug.LogError("[BaronShopCustomizer] Spawn placement failed: " + error);
                                        if (spawnedItem != null && !spawnedItem.IsHeld() && spawnedItem.owner == Item.Owner.Shopkeeper) spawnedItem.Despawn();
                                    }
                                    finally { if (generation == restockGeneration) pendingSpawns = Math.Max(0, pendingSpawns - 1); }
                                }, targetHolder.transform.position, targetHolder.transform.rotation, null, false);
                            yield return null;
                        }
                    }
                    else if (isApparelDisplay && shelves.Count > 0 && apparelPool.Count > 0)
                    {
                        for (int s = 0; s < shelves.Count; s++)
                        {
                            ItemShelf shelf = shelves[s];
                            if (shelf == null) continue;

                            List<Transform> spots = shelf.shelfSpots != null ? new List<Transform>(new HashSet<Transform>(shelf.shelfSpots)) : new List<Transform>();

                            for (int sp = 0; sp < spots.Count; sp++)
                            {
                                Transform spot = spots[sp];
                                if (spot == null) continue;
                                bool occupied = false;
                                if (display.displayItems != null)
                                    foreach (var existing in display.displayItems)
                                        if (existing != null && Vector3.Distance(existing.transform.position, spot.position) < 0.2f) { occupied = true; break; }
                                if (occupied) continue;

                                ItemData picked = apparelPool[apparelIndex % apparelPool.Count];
                                apparelIndex++;

                                ShopDisplay targetDisplay = display;
                                Transform targetSpot = spot;

                                pendingSpawns++;
                                picked.SpawnAsync(spawnedItem =>
                                {
                                    try
                                    {
                                    if (generation != restockGeneration || shop == null || activeShop != shop || !isActiveAndEnabled || targetDisplay == null)
                                    { if (spawnedItem != null) spawnedItem.Despawn(); return; }
                                    if (targetSpot == null) { if (spawnedItem != null) spawnedItem.Despawn(); return; }
                                    if (spawnedItem != null)
                                    {
                                        spawnedItem.isUsed = false;
                                        spawnedItem.SetOwner(Item.Owner.Shopkeeper);
                                        spawnedItem.transform.position = targetSpot.position;
                                        spawnedItem.transform.rotation = targetSpot.rotation;
                                        if (spawnedItem.physicBody != null)
                                        {
                                            spawnedItem.physicBody.velocity = Vector3.zero;
                                            spawnedItem.physicBody.angularVelocity = Vector3.zero;
                                        }
                                        ApplyPriceToItem(spawnedItem);
                                        if (activeShop != null)
                                        {
                                            activeShop.AddPriceTags(spawnedItem);
                                        }
                                        if (targetDisplay != null && targetDisplay.displayItems != null)
                                        {
                                            targetDisplay.displayItems.Add(spawnedItem);
                                        }
                                    }
                                
                                    }
                                    catch (Exception error)
                                    {
                                        Debug.LogError("[BaronShopCustomizer] Spawn placement failed: " + error);
                                        if (spawnedItem != null && !spawnedItem.IsHeld() && spawnedItem.owner == Item.Owner.Shopkeeper) spawnedItem.Despawn();
                                    }
                                    finally { if (generation == restockGeneration) pendingSpawns = Math.Max(0, pendingSpawns - 1); }
                                }, targetSpot.position, targetSpot.rotation, null, false);
                            }
                        }
                    }
                    else if (isConsumableDisplay && baskets.Count > 0 && consumablePool.Count > 0)
                    {
                        for (int b = 0; b < baskets.Count; b++)
                        {
                            ItemBasket basket = baskets[b];
                            if (basket == null) continue;

                            int preserved = 0;
                            if (display.displayItems != null)
                                foreach (var existing in display.displayItems)
                                    if (existing != null && Vector3.Distance(existing.transform.position, basket.transform.position) < 1f) preserved++;
                            int countToSpawn = Math.Max(0, Math.Min(basket.GetMaxItems(), 4) - preserved);
                            for (int c = 0; c < countToSpawn; c++)
                            {
                                ItemData picked = consumablePool[consumableIndex % consumablePool.Count];
                                consumableIndex++;

                                int heightIdx = c;
                                ItemBasket targetBasket = basket;
                                Vector3 spawnPos = targetBasket.GetRandomPositionAtHeight(heightIdx);
                                ShopDisplay targetDisplay = display;

                                pendingSpawns++;
                                picked.SpawnAsync(spawnedItem =>
                                {
                                    try
                                    {
                                    if (generation != restockGeneration || shop == null || activeShop != shop || !isActiveAndEnabled || targetDisplay == null)
                                    { if (spawnedItem != null) spawnedItem.Despawn(); return; }
                                    if (spawnedItem != null)
                                    {
                                        spawnedItem.isUsed = false;
                                        spawnedItem.SetOwner(Item.Owner.Shopkeeper);
                                        spawnedItem.transform.position = spawnPos;
                                        spawnedItem.transform.rotation = UnityEngine.Random.rotation;
                                        if (spawnedItem.physicBody != null)
                                        {
                                            spawnedItem.physicBody.velocity = Vector3.zero;
                                            spawnedItem.physicBody.angularVelocity = Vector3.zero;
                                        }
                                        ApplyPriceToItem(spawnedItem);
                                        if (activeShop != null)
                                        {
                                            activeShop.AddPriceTags(spawnedItem);
                                        }
                                        if (targetDisplay != null && targetDisplay.displayItems != null)
                                        {
                                            targetDisplay.displayItems.Add(spawnedItem);
                                        }
                                    }
                                
                                    }
                                    catch (Exception error)
                                    {
                                        Debug.LogError("[BaronShopCustomizer] Spawn placement failed: " + error);
                                        if (spawnedItem != null && !spawnedItem.IsHeld() && spawnedItem.owner == Item.Owner.Shopkeeper) spawnedItem.Despawn();
                                    }
                                    finally { if (generation == restockGeneration) pendingSpawns = Math.Max(0, pendingSpawns - 1); }
                                }, spawnPos, Quaternion.identity, null, false);
                            }
                        }
                    }
                }
            }

            yield return new WaitForSecondsRealtime(0.35f);
            UpdateAllPriceTags();
        }

        private bool CanRefill(Holder holder, List<ItemData> pool)
        {
            if (holder == null) return false;
            if (holder.data == null) holder.TryLoadFromID();
            if (holder.data == null || holder.data.slots == null) return false;
            foreach (var data in pool)
                if (data != null && !string.IsNullOrEmpty(data.slot) && holder.data.slots.Contains(data.slot)) return true;
            return false;
        }

        private List<Holder> GetDisplayHolders(ShopDisplay display)
        {
            List<Holder> list = new List<Holder>();
            if (display == null) return list;

            if (display.holders != null && display.holders.Count > 0)
            {
                list.AddRange(display.holders);
            }
            else
            {
                list.AddRange(display.GetComponentsInChildren<Holder>());
                if (display.includeTransforms != null)
                {
                    for (int i = 0; i < display.includeTransforms.Count; i++)
                    {
                        if (display.includeTransforms[i] != null)
                        {
                            list.AddRange(display.includeTransforms[i].GetComponentsInChildren<Holder>());
                        }
                    }
                }
            }
            return new List<Holder>(new HashSet<Holder>(list));
        }

        private List<ItemShelf> GetDisplayShelves(ShopDisplay display)
        {
            List<ItemShelf> list = new List<ItemShelf>();
            if (display == null) return list;

            if (display.shelves != null && display.shelves.Count > 0)
            {
                list.AddRange(display.shelves);
            }
            else
            {
                list.AddRange(display.GetComponentsInChildren<ItemShelf>());
                if (display.includeTransforms != null)
                {
                    for (int i = 0; i < display.includeTransforms.Count; i++)
                    {
                        if (display.includeTransforms[i] != null)
                        {
                            list.AddRange(display.includeTransforms[i].GetComponentsInChildren<ItemShelf>());
                        }
                    }
                }
            }
            return new List<ItemShelf>(new HashSet<ItemShelf>(list));
        }

        private List<ItemBasket> GetDisplayBaskets(ShopDisplay display)
        {
            List<ItemBasket> list = new List<ItemBasket>();
            if (display == null) return list;

            if (display.baskets != null && display.baskets.Count > 0)
            {
                list.AddRange(display.baskets);
            }
            else
            {
                list.AddRange(display.GetComponentsInChildren<ItemBasket>());
                if (display.includeTransforms != null)
                {
                    for (int i = 0; i < display.includeTransforms.Count; i++)
                    {
                        if (display.includeTransforms[i] != null)
                        {
                            list.AddRange(display.includeTransforms[i].GetComponentsInChildren<ItemBasket>());
                        }
                    }
                }
            }
            return new List<ItemBasket>(new HashSet<ItemBasket>(list));
        }

        private List<ItemData> FilterWeapons(List<ItemData> all, int tierFilter, string categoryFilter)
        {
            List<ItemData> list = new List<ItemData>();
            for (int i = 0; i < all.Count; i++)
            {
                ItemData item = all[i];
                if (item == null || item.value <= 0 || !IsWeapon(item)) continue;

                if (tierFilter >= 0 && item.tier != tierFilter) continue;

                if (categoryFilter != "All" && !MatchesWeaponSubcategory(item, categoryFilter)) continue;

                list.Add(item);
            }
            return list;
        }

        private List<ItemData> FilterApparel(List<ItemData> all, int tierFilter, string typeFilter)
        {
            List<ItemData> list = new List<ItemData>();
            for (int i = 0; i < all.Count; i++)
            {
                ItemData item = all[i];
                if (item == null || item.value <= 0) continue;

                bool isArm = IsArmor(item);
                bool isClo = IsClothes(item);
                if (!isArm && !isClo) continue;

                if (tierFilter >= 0 && item.tier != tierFilter) continue;

                if (typeFilter == "Armor" && !isArm) continue;
                if (typeFilter == "Clothes" && !isClo) continue;

                list.Add(item);
            }
            return list;
        }

        private List<ItemData> FilterConsumables(List<ItemData> all, string typeFilter)
        {
            List<ItemData> list = new List<ItemData>();
            for (int i = 0; i < all.Count; i++)
            {
                ItemData item = all[i];
                if (item == null || item.value <= 0 || !IsConsumable(item)) continue;

                if (typeFilter != "All")
                {
                    string text = (item.id + " " + item.displayName + " " + item.category).ToLowerInvariant();
                    if (typeFilter == "Potions" && (text.IndexOf("potion") < 0 && text.IndexOf("flask") < 0 && text.IndexOf("vial") < 0)) continue;
                    if (typeFilter == "Food" && (text.IndexOf("apple") < 0 && text.IndexOf("bread") < 0 && text.IndexOf("meat") < 0 && text.IndexOf("food") < 0)) continue;
                }

                list.Add(item);
            }
            return list;
        }

        private bool MatchesWeaponSubcategory(ItemData item, string subcategory)
        {
            string text = (item.id + " " + item.displayName + " " + item.category).ToLowerInvariant();
            switch (subcategory)
            {
                case "Swords":
                    return text.IndexOf("sword") >= 0 || text.IndexOf("falchion") >= 0 || text.IndexOf("claymore") >= 0 || text.IndexOf("rapier") >= 0 || text.IndexOf("blade") >= 0 || text.IndexOf("katana") >= 0 || text.IndexOf("saber") >= 0;
                case "Daggers":
                    return text.IndexOf("dagger") >= 0 || text.IndexOf("knife") >= 0 || text.IndexOf("shiv") >= 0 || text.IndexOf("kunai") >= 0;
                case "Axes":
                    return text.IndexOf("axe") >= 0 || text.IndexOf("hatchet") >= 0 || text.IndexOf("bardiche") >= 0;
                case "Maces":
                    return text.IndexOf("mace") >= 0 || text.IndexOf("hammer") >= 0 || text.IndexOf("flail") >= 0 || text.IndexOf("maul") >= 0 || text.IndexOf("club") >= 0;
                case "Polearms":
                    return text.IndexOf("spear") >= 0 || text.IndexOf("polearm") >= 0 || text.IndexOf("halberd") >= 0 || text.IndexOf("pike") >= 0 || text.IndexOf("lance") >= 0;
                case "Bows":
                    return text.IndexOf("bow") >= 0 || text.IndexOf("arrow") >= 0 || text.IndexOf("quiver") >= 0 || item.type == ItemData.Type.Quiver;
                case "Magic":
                    return text.IndexOf("staff") >= 0 || text.IndexOf("wand") >= 0 || text.IndexOf("crystal") >= 0 || text.IndexOf("spell") >= 0 || item.type == ItemData.Type.Spell || item.type == ItemData.Type.Crystal;
                case "Shields":
                    return text.IndexOf("shield") >= 0 || text.IndexOf("buckler") >= 0 || item.type == ItemData.Type.Shield;
                default:
                    return true;
            }
        }

        private bool IsWeapon(ItemData item)
        {
            if (item == null || item.value <= 0) return false;
            if (item.type == ItemData.Type.Weapon || item.type == ItemData.Type.Shield || item.type == ItemData.Type.Quiver) return true;
            if (item.type == ItemData.Type.Wardrobe || item.type == ItemData.Type.Potion || item.type == ItemData.Type.Food || item.type == ItemData.Type.Spell) return false;
            string text = (item.id + " " + item.displayName + " " + item.category).ToLowerInvariant();
            if (text.IndexOf("armor") >= 0 || text.IndexOf("wardrobe") >= 0 || text.IndexOf("apparel") >= 0 || text.IndexOf("potion") >= 0 || text.IndexOf("apple") >= 0) return false;
            return text.IndexOf("weapon") >= 0 || text.IndexOf("sword") >= 0 || text.IndexOf("dagger") >= 0 || text.IndexOf("axe") >= 0 || text.IndexOf("mace") >= 0 || text.IndexOf("hammer") >= 0 || text.IndexOf("spear") >= 0 || text.IndexOf("bow") >= 0 || text.IndexOf("shield") >= 0 || text.IndexOf("staff") >= 0;
        }

        private bool IsApparel(ItemData item)
        {
            if (item == null || item.value <= 0) return false;
            if (item.type == ItemData.Type.Wardrobe) return true;
            string text = (item.id + " " + item.displayName + " " + item.category).ToLowerInvariant();
            return text.IndexOf("armor") >= 0 || text.IndexOf("wardrobe") >= 0 || text.IndexOf("apparel") >= 0 || text.IndexOf("cloth") >= 0;
        }

        private bool IsArmor(ItemData item)
        {
            if (!IsApparel(item)) return false;
            string text = (item.id + " " + item.displayName + " " + item.category + " " + item.slot).ToLowerInvariant();
            return text.IndexOf("plate") >= 0 || text.IndexOf("chainmail") >= 0 || text.IndexOf("cuirass") >= 0 || text.IndexOf("gauntlet") >= 0 || text.IndexOf("greave") >= 0 || text.IndexOf("iron") >= 0 || text.IndexOf("steel") >= 0 || text.IndexOf("heavy") >= 0 || text.IndexOf("metal") >= 0 || text.IndexOf("armor") >= 0;
        }

        private bool IsClothes(ItemData item)
        {
            if (!IsApparel(item)) return false;
            return !IsArmor(item);
        }

        private bool IsConsumable(ItemData item)
        {
            if (item == null || item.value <= 0) return false;
            if (item.type == ItemData.Type.Potion || item.type == ItemData.Type.Food) return true;
            string text = (item.id + " " + item.displayName + " " + item.category).ToLowerInvariant();
            return text.IndexOf("potion") >= 0 || text.IndexOf("flask") >= 0 || text.IndexOf("vial") >= 0 || text.IndexOf("apple") >= 0 || text.IndexOf("bread") >= 0 || text.IndexOf("food") >= 0 || text.IndexOf("consumable") >= 0;
        }

        private void ShuffleList<T>(List<T> list)
        {
            if (list == null || list.Count <= 1) return;
            for (int i = list.Count - 1; i > 0; i--)
            {
                int rnd = UnityEngine.Random.Range(0, i + 1);
                T temp = list[i];
                list[i] = list[rnd];
                list[rnd] = temp;
            }
        }
    }
}
