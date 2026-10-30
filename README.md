# Baron Shop Customizer

Controle os filtros, o estoque e os preços da loja do Barão em Blade & Sorcery: Nomad.

**Versão:** 1.1.1 · **Tecnologia:** C# / .NET Standard 2.1 · **Licença:** [MIT](LICENSE)

## Funcionalidades

- Multiplicador de compra geral ou separado para armas, armaduras, roupas e consumíveis.
- Multiplicador de venda de itens do jogador.
- Filtros por categoria e por tier, incluindo opções de 0 a 6. A disponibilidade real depende do catálogo instalado.
- Atualização manual da loja e opção de atualizar ao alterar configurações.
- Interface pelo livro de opções do jogo, sem aplicativo externo.

## Uso no jogo

Entre na loja do Barão e abra **Mod Options → Baron Shop Customizer** no livro. Escolha os modos de preço e filtro e os multiplicadores desejados. Use **Atualizar Loja (Fast Refresh)** para repopular os suportes. Compras e vendas continuam sujeitas ao salvamento normal do jogo.

| Opção | Uso |
| --- | --- |
| Modo de preço de compra | Geral ou individual por categoria |
| Filtros de categoria / tier | Restringem o catálogo usado na reposição |
| Multiplicador de venda | Ajusta o valor recebido pelos itens vendidos |
| Fast Refresh | Solicita uma nova seleção dentro dos filtros |

## Compilar a partir do código

Pré-requisitos: Windows com PowerShell, SDK .NET 8 ou superior e DLLs de referência compatíveis de ThunderRoad/Unity, obtidas de uma instalação ou SDK autorizado. O jogo não está incluído. Não use stubs para produzir um mod jogável.

1. Clone somente este repositório.
2. Coloque as referências em uma pasta externa ou em `sdk/` (ignorada pelo Git). Ela deve conter `ThunderRoad.dll`, `Newtonsoft.Json.dll`, `UnityEngine.dll` e os módulos `UnityEngine.*.dll` correspondentes.
3. Na raiz do clone, execute:

```powershell
./build.ps1 -SdkDir (Read-Host 'Pasta das DLLs de referencia')
```

Também é possível definir `MOD_SDK_DIR` ou usar `./build.ps1` com as referências em `sdk/`. O caminho é resolvido em tempo de execução e não depende da máquina do autor.

Para executar os testes de lógica sem jogo ou referências proprietárias:

```powershell
dotnet run --project tests/LogicTests.csproj -c Release
```

O build cria `bin/`, `obj/` e `dist/` automaticamente. A saída instalável fica em `dist/BaronShopCustomizer/`. Copie somente essa pasta para a pasta `Mods` da instalação do jogo, usando o gerenciador de arquivos do dispositivo. Fontes, testes e bibliotecas de referência não precisam ser instalados no headset. Reinicie o jogo e evite duas cópias do mesmo mod.

## Comandos de desenvolvimento

| Comando / parâmetro | Resultado |
| --- | --- |
| `./build.ps1 -SdkDir <pasta>` | Compila Release, executa testes e gera o pacote |
| `MOD_SDK_DIR` | Define a pasta de referências sem editar o projeto |
| `dotnet run --project tests/LogicTests.csproj -c Release` | Executa os testes públicos de lógica |

## Privacidade

O código não implementa telemetria, autenticação ou envio de dados a serviços externos. As opções são expostas pelo sistema de mods do jogo. Binários, símbolos de depuração, referências proprietárias, logs, credenciais e arquivos locais estão excluídos do Git. Não há caminhos pessoais nos arquivos preparados para publicação.

## Estrutura do repositório

| Pasta / arquivo | Finalidade |
| --- | --- |
| `src/` | Código C# do mod |
| `tests/` | Testes públicos de lógica sem dependência do jogo |
| `manifest.json` | Metadados lidos pelo jogo |
| `BaronShopCustomizer.csproj` e `build.ps1` | Compilação e empacotamento independentes |
| `docs/AUDIT.md` | Auditoria e limites da validação |
| `docs/THIRD_PARTY.md` | Dependências e direitos de terceiros |

## Validação e limites

Build Release e 7 verificações de seleção de estoque: coleção vazia, entradas nulas, ausência de item compatível, retorno circular e offsets negativos/grandes.

A compilação local usa referências reais da instalação PC disponível. Ela apresenta dois avisos MSB3277 de conflito entre versões de `System.Net.Http` e `System.IO.Compression` exigidas pelo SDK. Os avisos não foram ocultados. Não houve execução no Unity/IL2CPP, teste visual, medição de FPS ou teste no Quest nesta preparação. A compatibilidade com Nomad 1.3.1 é o alvo informado pelo projeto, ainda dependente de validação no dispositivo. O campo `GameVersion` do manifesto foi preservado em `1.0.0.0`; ele não representa uma confirmação de compatibilidade com todas as versões do jogo.

## Licença

Código sob [MIT](LICENSE), escolhida pela permissão simples de uso, modificação e redistribuição. Jogo, SDK e outros materiais de terceiros não são relicenciados; consulte [as notas de dependências](docs/THIRD_PARTY.md).
