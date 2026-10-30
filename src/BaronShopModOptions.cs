using System;
using ThunderRoad;

namespace BaronShopCustomizer
{
    public static class BaronShopModOptions
    {
        public static string priceMode = "General";
        public static float generalBuyMultiplier = 1.0f;

        public static string filterMode = "General";
        public static int generalTierFilter = -1;

        public static float generalSellMultiplier = 1.0f;

        public static float weaponBuyMultiplier = 1.0f;
        public static float armorBuyMultiplier = 1.0f;
        public static float clothesBuyMultiplier = 1.0f;
        public static float consumableBuyMultiplier = 1.0f;

        public static string weaponCategoryFilter = "All";
        public static int weaponTierFilter = -1;

        public static string apparelTypeFilter = "All";
        public static int apparelTierFilter = -1;

        public static string consumableTypeFilter = "All";

        public static bool autoRefreshOnChange = true;

        public static Action OnOptionsChanged;
        public static Action OnFastRefreshRequested;

        [ModOptionOrder(1)]
        public static ModOptionString[] PriceModeOptions()
        {
            return new ModOptionString[]
            {
                new ModOptionString("Geral (Todos os Itens)", "General"),
                new ModOptionString("Individual por Categoria", "PerCategory")
            };
        }

        [ModOptionButton]
        [ModOptionCategory("Baron Shop Customizer", 1)]
        [ModOption("Modo de Preco de Compra", "Alterna entre multiplicador geral ou individual", "PriceModeOptions", defaultValueIndex = 0)]
        public static void SetPriceMode(string val)
        {
            priceMode = val;
            NotifyChange();
        }

        [ModOptionOrder(2)]
        public static ModOptionFloat[] GeneralBuyMultiplierOptions()
        {
            return new ModOptionFloat[]
            {
                new ModOptionFloat("1.0x (Normal)", 1.0f),
                new ModOptionFloat("0.0x (Gratis / Tudo Free)", 0.0f),
                new ModOptionFloat("0.25x (-75%)", 0.25f),
                new ModOptionFloat("0.5x (Metade)", 0.5f),
                new ModOptionFloat("0.8x (-20%)", 0.8f),
                new ModOptionFloat("1.2x (+20%)", 1.2f),
                new ModOptionFloat("1.5x (+50%)", 1.5f),
                new ModOptionFloat("2.0x (Dobro)", 2.0f)
            };
        }

        [ModOptionButton]
        [ModOptionCategory("Baron Shop Customizer", 1)]
        [ModOption("Compra Geral: Todos os Itens", "Multiplicador de compra aplicado a toda a loja", "GeneralBuyMultiplierOptions", defaultValueIndex = 0)]
        public static void SetGeneralBuyMultiplier(float val)
        {
            generalBuyMultiplier = val;
            NotifyChange();
        }

        [ModOptionOrder(3)]
        public static ModOptionString[] FilterModeOptions()
        {
            return new ModOptionString[]
            {
                new ModOptionString("Geral (Sorteio Padrao por Tier)", "General"),
                new ModOptionString("Individual por Categoria", "PerCategory")
            };
        }

        [ModOptionButton]
        [ModOptionCategory("Baron Shop Customizer", 1)]
        [ModOption("Modo de Filtro do Estoque", "Alterna entre controle geral por tier ou filtros individuais", "FilterModeOptions", defaultValueIndex = 0)]
        public static void SetFilterMode(string val)
        {
            filterMode = val;
            if (autoRefreshOnChange)
            {
                TriggerRefresh();
            }
        }

        [ModOptionOrder(4)]
        public static ModOptionInt[] GeneralTierOptions()
        {
            return new ModOptionInt[]
            {
                new ModOptionInt("Qualquer Tier (Todos)", -1),
                new ModOptionInt("Tier 0 (Inicial / Rustico)", 0),
                new ModOptionInt("Tier 1", 1),
                new ModOptionInt("Tier 2", 2),
                new ModOptionInt("Tier 3", 3),
                new ModOptionInt("Tier 4", 4),
                new ModOptionInt("Tier 5", 5),
                new ModOptionInt("Tier 6 (Max / Mitico)", 6)
            };
        }

        [ModOptionButton]
        [ModOptionCategory("Baron Shop Customizer", 1)]
        [ModOption("Nivel Geral (Tier de Todos os Itens)", "Fixa o tier de todas as armas e roupas mantendo a variedade", "GeneralTierOptions", defaultValueIndex = 0)]
        public static void SetGeneralTierFilter(int val)
        {
            generalTierFilter = val;
            if (autoRefreshOnChange)
            {
                TriggerRefresh();
            }
        }

        [ModOptionOrder(14)]
        public static ModOptionFloat[] GeneralSellMultiplierOptions()
        {
            return new ModOptionFloat[]
            {
                new ModOptionFloat("1.0x (Normal)", 1.0f),
                new ModOptionFloat("0.0x (Sem Valor)", 0.0f),
                new ModOptionFloat("0.5x (Metade)", 0.5f),
                new ModOptionFloat("1.5x (+50%)", 1.5f),
                new ModOptionFloat("2.0x (2x)", 2.0f),
                new ModOptionFloat("2.5x (2.5x)", 2.5f),
                new ModOptionFloat("3.0x (3x)", 3.0f),
                new ModOptionFloat("3.5x (3.5x)", 3.5f),
                new ModOptionFloat("5.0x (5x)", 5.0f),
                new ModOptionFloat("7.5x (7.5x)", 7.5f),
                new ModOptionFloat("10.0x (10x Max)", 10.0f)
            };
        }

        [ModOptionButton]
        [ModOptionCategory("Baron Shop Customizer", 1)]
        [ModOption("Venda: Itens do Jogador", "Multiplicador de venda dos itens para o Barao", "GeneralSellMultiplierOptions", defaultValueIndex = 0)]
        public static void SetGeneralSellMultiplier(float val)
        {
            generalSellMultiplier = val;
            NotifyChange();
        }

        [ModOptionOrder(10)]
        public static ModOptionFloat[] WeaponBuyMultiplierOptions()
        {
            return new ModOptionFloat[]
            {
                new ModOptionFloat("1.0x (Normal)", 1.0f),
                new ModOptionFloat("0.0x (Gratis)", 0.0f),
                new ModOptionFloat("0.25x (-75%)", 0.25f),
                new ModOptionFloat("0.5x (Metade)", 0.5f),
                new ModOptionFloat("0.8x (-20%)", 0.8f),
                new ModOptionFloat("1.2x (+20%)", 1.2f),
                new ModOptionFloat("1.5x (+50%)", 1.5f),
                new ModOptionFloat("2.0x (Dobro)", 2.0f)
            };
        }

        [ModOptionButton]
        [ModOptionCategory("Baron Shop Customizer", 1)]
        [ModOption("Cat. Compra: Armas", "Multiplicador individual para armas (Modo Por Categoria)", "WeaponBuyMultiplierOptions", defaultValueIndex = 0)]
        public static void SetWeaponBuyMultiplier(float val)
        {
            weaponBuyMultiplier = val;
            NotifyChange();
        }

        [ModOptionOrder(11)]
        public static ModOptionFloat[] ArmorBuyMultiplierOptions()
        {
            return new ModOptionFloat[]
            {
                new ModOptionFloat("1.0x (Normal)", 1.0f),
                new ModOptionFloat("0.0x (Gratis)", 0.0f),
                new ModOptionFloat("0.25x (-75%)", 0.25f),
                new ModOptionFloat("0.4x (-60%)", 0.4f),
                new ModOptionFloat("0.6x (-40%)", 0.6f),
                new ModOptionFloat("0.8x (-20%)", 0.8f),
                new ModOptionFloat("1.2x (+20%)", 1.2f),
                new ModOptionFloat("1.5x (+50%)", 1.5f),
                new ModOptionFloat("2.0x (Dobro)", 2.0f)
            };
        }

        [ModOptionButton]
        [ModOptionCategory("Baron Shop Customizer", 1)]
        [ModOption("Cat. Compra: Armaduras", "Multiplicador individual para armaduras (Modo Por Categoria)", "ArmorBuyMultiplierOptions", defaultValueIndex = 0)]
        public static void SetArmorBuyMultiplier(float val)
        {
            armorBuyMultiplier = val;
            NotifyChange();
        }

        [ModOptionOrder(12)]
        public static ModOptionFloat[] ClothesBuyMultiplierOptions()
        {
            return new ModOptionFloat[]
            {
                new ModOptionFloat("1.0x (Normal)", 1.0f),
                new ModOptionFloat("0.0x (Gratis)", 0.0f),
                new ModOptionFloat("0.25x (-75%)", 0.25f),
                new ModOptionFloat("0.4x (-60%)", 0.4f),
                new ModOptionFloat("0.6x (-40%)", 0.6f),
                new ModOptionFloat("0.8x (-20%)", 0.8f),
                new ModOptionFloat("1.2x (+20%)", 1.2f),
                new ModOptionFloat("1.5x (+50%)", 1.5f),
                new ModOptionFloat("2.0x (Dobro)", 2.0f)
            };
        }

        [ModOptionButton]
        [ModOptionCategory("Baron Shop Customizer", 1)]
        [ModOption("Cat. Compra: Roupas", "Multiplicador individual para roupas (Modo Por Categoria)", "ClothesBuyMultiplierOptions", defaultValueIndex = 0)]
        public static void SetClothesBuyMultiplier(float val)
        {
            clothesBuyMultiplier = val;
            NotifyChange();
        }

        [ModOptionOrder(13)]
        public static ModOptionFloat[] ConsumableBuyMultiplierOptions()
        {
            return new ModOptionFloat[]
            {
                new ModOptionFloat("1.0x (Normal)", 1.0f),
                new ModOptionFloat("0.0x (Gratis)", 0.0f),
                new ModOptionFloat("0.5x (Metade)", 0.5f),
                new ModOptionFloat("1.5x (+50%)", 1.5f),
                new ModOptionFloat("2.0x (Dobro)", 2.0f)
            };
        }

        [ModOptionButton]
        [ModOptionCategory("Baron Shop Customizer", 1)]
        [ModOption("Cat. Compra: Consumiveis", "Multiplicador individual para pocoes e comidas", "ConsumableBuyMultiplierOptions", defaultValueIndex = 0)]
        public static void SetConsumableBuyMultiplier(float val)
        {
            consumableBuyMultiplier = val;
            NotifyChange();
        }

        [ModOptionOrder(5)]
        public static ModOptionString[] WeaponCategoryOptions()
        {
            return new ModOptionString[]
            {
                new ModOptionString("Todas as Armas", "All"),
                new ModOptionString("Espadas", "Swords"),
                new ModOptionString("Adagas", "Daggers"),
                new ModOptionString("Machados", "Axes"),
                new ModOptionString("Marretas e Martelos", "Maces"),
                new ModOptionString("Lancas e Hastes", "Polearms"),
                new ModOptionString("Arcos e Projetil", "Bows"),
                new ModOptionString("Magia e Cajados", "Magic"),
                new ModOptionString("Escudos", "Shields")
            };
        }

        [ModOptionButton]
        [ModOptionCategory("Baron Shop Customizer", 1)]
        [ModOption("Cat. Arma: Tipo Especifico", "Filtra quais armas aparecem (Modo Por Categoria)", "WeaponCategoryOptions", defaultValueIndex = 0)]
        public static void SetWeaponCategoryFilter(string val)
        {
            weaponCategoryFilter = val;
            if (autoRefreshOnChange)
            {
                TriggerRefresh();
            }
        }

        [ModOptionOrder(6)]
        public static ModOptionInt[] WeaponTierOptions()
        {
            return new ModOptionInt[]
            {
                new ModOptionInt("Qualquer Tier (Todos)", -1),
                new ModOptionInt("Tier 0 (Inicial / Rustico)", 0),
                new ModOptionInt("Tier 1", 1),
                new ModOptionInt("Tier 2", 2),
                new ModOptionInt("Tier 3", 3),
                new ModOptionInt("Tier 4", 4),
                new ModOptionInt("Tier 5", 5),
                new ModOptionInt("Tier 6 (Max / Mitico)", 6)
            };
        }

        [ModOptionButton]
        [ModOptionCategory("Baron Shop Customizer", 1)]
        [ModOption("Cat. Arma: Nivel Especifico", "Filtra o tier das armas (Modo Por Categoria)", "WeaponTierOptions", defaultValueIndex = 0)]
        public static void SetWeaponTierFilter(int val)
        {
            weaponTierFilter = val;
            if (autoRefreshOnChange)
            {
                TriggerRefresh();
            }
        }

        [ModOptionOrder(7)]
        public static ModOptionString[] ApparelTypeOptions()
        {
            return new ModOptionString[]
            {
                new ModOptionString("Todos (Armaduras e Roupas)", "All"),
                new ModOptionString("Apenas Armaduras", "Armor"),
                new ModOptionString("Apenas Roupas", "Clothes")
            };
        }

        [ModOptionButton]
        [ModOptionCategory("Baron Shop Customizer", 1)]
        [ModOption("Cat. Vestuario: Tipo Especifico", "Filtra armaduras vs roupas (Modo Por Categoria)", "ApparelTypeOptions", defaultValueIndex = 0)]
        public static void SetApparelTypeFilter(string val)
        {
            apparelTypeFilter = val;
            if (autoRefreshOnChange)
            {
                TriggerRefresh();
            }
        }

        [ModOptionOrder(8)]
        public static ModOptionInt[] ApparelTierOptions()
        {
            return new ModOptionInt[]
            {
                new ModOptionInt("Qualquer Tier (Todos)", -1),
                new ModOptionInt("Tier 0 (Trapos / Iniciais)", 0),
                new ModOptionInt("Tier 1", 1),
                new ModOptionInt("Tier 2", 2),
                new ModOptionInt("Tier 3", 3),
                new ModOptionInt("Tier 4", 4),
                new ModOptionInt("Tier 5", 5),
                new ModOptionInt("Tier 6 (Max)", 6)
            };
        }

        [ModOptionButton]
        [ModOptionCategory("Baron Shop Customizer", 1)]
        [ModOption("Cat. Vestuario: Nivel Especifico", "Filtra o tier do vestuario (Modo Por Categoria)", "ApparelTierOptions", defaultValueIndex = 0)]
        public static void SetApparelTierFilter(int val)
        {
            apparelTierFilter = val;
            if (autoRefreshOnChange)
            {
                TriggerRefresh();
            }
        }

        [ModOptionOrder(9)]
        public static ModOptionString[] ConsumableTypeOptions()
        {
            return new ModOptionString[]
            {
                new ModOptionString("Todos os Consumiveis", "All"),
                new ModOptionString("Pocoes", "Potions"),
                new ModOptionString("Alimentos", "Food")
            };
        }

        [ModOptionButton]
        [ModOptionCategory("Baron Shop Customizer", 1)]
        [ModOption("Cat. Consumiveis: Tipo Especifico", "Filtra os consumiveis nos cestos", "ConsumableTypeOptions", defaultValueIndex = 0)]
        public static void SetConsumableTypeFilter(string val)
        {
            consumableTypeFilter = val;
            if (autoRefreshOnChange)
            {
                TriggerRefresh();
            }
        }

        [ModOptionOrder(15)]
        public static ModOptionBool[] AutoRefreshOptions()
        {
            return new ModOptionBool[]
            {
                new ModOptionBool("Ativado (Instantaneo)", true),
                new ModOptionBool("Desativado (Manual)", false)
            };
        }

        [ModOptionButton]
        [ModOptionCategory("Baron Shop Customizer", 1)]
        [ModOption("Auto-Restock ao Mudar Opcao", "Recarrega os suportes ao alterar qualquer filtro", "AutoRefreshOptions", defaultValueIndex = 0)]
        public static void SetAutoRefresh(bool val)
        {
            autoRefreshOnChange = val;
        }

        [ModOptionOrder(16)]
        public static ModOptionString[] FastRefreshOptions()
        {
            return new ModOptionString[]
            {
                new ModOptionString("Atualizar Agora", "Refresh")
            };
        }

        [ModOptionButton]
        [ModOptionCategory("Baron Shop Customizer", 1)]
        [ModOption("Atualizar Loja (Fast Refresh)", "Limpa e sorteia novos itens imediatamente", "FastRefreshOptions", defaultValueIndex = 0)]
        public static void ActionFastRefresh(string val)
        {
            TriggerRefresh();
        }

        private static void NotifyChange()
        {
            try
            {
                OnOptionsChanged?.Invoke();
            }
            catch (Exception error)
            {
                UnityEngine.Debug.LogError("[BaronShopCustomizer] Option action failed: " + error);
            }
        }

        private static void TriggerRefresh()
        {
            try
            {
                OnFastRefreshRequested?.Invoke();
            }
            catch (Exception error)
            {
                UnityEngine.Debug.LogError("[BaronShopCustomizer] Option action failed: " + error);
            }
        }
    }
}
