<h1 align="center">Baron Shop Customizer</h1>

<p align="center">Escolha o estoque. Ajuste os preços. Personalize a loja do Barão em Blade &amp; Sorcery: Nomad.</p>

<p align="center">
  <a href="https://github.com/yGuilhermy/BaronShopCustomizer/releases/tag/v1.1.1"><strong>Baixar v1.1.1</strong></a> ·
  <a href="#instalar">Instalação</a> ·
  <a href="#usar">Como usar</a> ·
  <a href="#perguntas-frequentes">FAQ</a> ·
  <a href="https://github.com/yGuilhermy/BaronShopCustomizer/issues">Reportar problema</a>
</p>

<p align="center">C# / .NET Standard 2.1 · Código aberto sob MIT · Pré-release</p>

---

Controle quais itens aparecem na loja e quanto você paga ou recebe por eles. Combine filtros de categoria e tier, aplique multiplicadores gerais ou individuais e solicite uma nova seleção de estoque pelo livro de opções do jogo.

> [!IMPORTANT]
> Esta versão é uma **pré-release**, com alvo em Nomad 1.3.1 e validação no Quest ainda pendente. A disponibilidade dos itens depende do catálogo instalado.

## Instalar

Para instalar o pacote pronto, você precisa de Blade & Sorcery: Nomad com suporte a mods de script compatíveis. Não é necessário ter .NET ou compilar o projeto no computador.

### 1. Baixe o pacote

| Arquivo | Finalidade |
| --- | --- |
| [BaronShopCustomizer-v1.1.1.zip](https://github.com/yGuilhermy/BaronShopCustomizer/releases/download/v1.1.1/BaronShopCustomizer-v1.1.1.zip) | Mod pronto para instalação |
| [BaronShopCustomizer-v1.1.1.zip.sha256](https://github.com/yGuilhermy/BaronShopCustomizer/releases/download/v1.1.1/BaronShopCustomizer-v1.1.1.zip.sha256) | Hash SHA-256 para conferir o download |

### 2. Copie para o jogo

1. Feche o jogo e extraia o ZIP.
2. Copie a pasta `BaronShopCustomizer` para a pasta `Mods` da instalação de Nomad.
3. Confira a estrutura abaixo e reinicie o jogo.

```text
Mods/
└── BaronShopCustomizer/
    ├── BaronShopCustomizer.dll
    ├── manifest.json
    ├── LICENSE
    ├── README.md
    └── THIRD_PARTY.md
```

Ao atualizar, substitua a instalação anterior e evite duas cópias do mod. `manifest.json` e a DLL devem ficar diretamente dentro de `BaronShopCustomizer/`, sem outra pasta de mesmo nome no meio.

## Usar

1. Carregue seu personagem e viaje até a loja do Barão.
2. Abra **Mod Options → Baron Shop Customizer** no livro.
3. Escolha os modos de preço e filtro e ajuste os valores desejados.
4. Use **Atualizar Loja (Fast Refresh)** para solicitar uma nova seleção de itens dentro dos filtros.

### Configurações principais

| Opção | O que controla |
| --- | --- |
| Modo de preço de compra | Um multiplicador geral ou valores separados por categoria |
| Multiplicador de venda | Quanto você recebe ao vender itens |
| Modo de filtro | Tier geral ou filtros individuais por categoria |
| Filtros de armas | Tipo de arma e tier desejado |
| Filtros de vestuário | Armaduras ou roupas e tier desejado |
| Filtros de consumíveis | Poções, alimentos ou todos os consumíveis |
| Auto-Restock ao Mudar Opção | Solicita reposição ao alterar configurações |
| Atualizar Loja (Fast Refresh) | Solicita uma nova seleção com as regras atuais |

### Exemplo: loja pela metade do preço

Escolha o modo de preço **Geral** e o multiplicador `0.5x`. Para restringir o estoque a um nível, escolha o modo de filtro **Geral**, selecione o tier e use **Fast Refresh**.

### Exemplo: selecionar apenas espadas

Escolha o filtro **Individual por Categoria**, selecione **Espadas** em tipo de arma e escolha o tier. As opções de vestuário e consumíveis continuam com seus próprios filtros.

## Recursos

| Recurso | O que oferece |
| --- | --- |
| Preços de compra | Ajuste geral ou separado para armas, armaduras, roupas e consumíveis |
| Valores de venda | Multiplicador para os itens vendidos ao Barão |
| Seleção de armas | Espadas, adagas, machados, marretas, hastes, arcos, cajados e escudos |
| Filtros de nível | Opções de tier 0 a 6, conforme o catálogo disponível |
| Atualização de estoque | Reposição manual e opção de atualizar ao mudar configurações |
| Menu dentro do jogo | Configuração pelo livro, sem aplicativo externo |

## Perguntas frequentes

### Escolher um tier cria itens que não existem no catálogo?

Não. O filtro trabalha com os itens disponíveis na instalação. A presença de uma opção no menu não garante que todas as categorias tenham itens daquele tier.

### Posso sortear outros itens sem mudar os filtros?

Sim. Use **Fast Refresh** novamente para solicitar uma nova seleção dentro das regras atuais. O resultado pode repetir itens quando o conjunto disponível é pequeno.

### As compras ficam no salvamento?

Compras e vendas continuam sujeitas ao salvamento normal do jogo. Considere os multiplicadores escolhidos antes de concluir uma transação.

### O mod não aparece no livro. O que conferir?

Verifique se a DLL e o manifesto estão diretamente na pasta do mod, se não há uma instalação duplicada e se a versão do jogo é compatível. Reinicie o jogo após instalar ou atualizar.

### Esta versão já foi testada no headset?

Ainda não. O build Release e 7 verificações de seleção de estoque passaram, inclusive em uma cópia isolada dos arquivos públicos. Isso não substitui testes de execução Unity/IL2CPP e de comportamento no Quest. Veja o [relatório de validação](docs/AUDIT.md).

## Desenvolvimento

Os testes de seleção de estoque podem ser executados sem instalar o jogo. Para compilar a DLL completa, são necessárias referências externas compatíveis de ThunderRoad e Unity.

<details>
<summary><strong>Pré-requisitos, compilação e testes</strong></summary>

### Pré-requisitos

- Windows com PowerShell.
- SDK .NET 8 ou superior.
- Referências obtidas de uma instalação ou SDK autorizado: `ThunderRoad.dll`, `Newtonsoft.Json.dll`, `UnityEngine.dll` e os módulos `UnityEngine.*.dll` correspondentes.

As bibliotecas do jogo não são distribuídas neste repositório. Stubs não substituem essas referências em um build jogável.

### Compilar

```powershell
git clone https://github.com/yGuilhermy/BaronShopCustomizer.git
cd BaronShopCustomizer
./build.ps1 -SdkDir (Read-Host 'Pasta das DLLs de referencia')
```

O script compila em Release, executa os testes e cria `dist/BaronShopCustomizer/`. Também aceita `MOD_SDK_DIR`; sem parâmetro ou variável, procura as referências em `sdk/`, relativa à raiz do projeto.

### Testar a lógica

```powershell
dotnet run --project tests/LogicTests.csproj -c Release
```

A suíte cobre coleção vazia, entradas nulas, ausência de item compatível, retorno circular e offsets negativos ou grandes.

### Limites do build verificado

As referências PC utilizadas geram dois avisos MSB3277, relacionados a `System.Net.Http` e `System.IO.Compression`. O build conclui sem erros, mas a compatibilidade binária com Nomad precisa de validação no destino. O manifesto mantém `GameVersion` em `1.0.0.0`; esse campo não comprova compatibilidade com outras versões.

</details>

## Documentação

| Documento | Conteúdo |
| --- | --- |
| [Notas do release](docs/RELEASE.md) | Pacote, instalação e limites da versão |
| [Dependências](docs/THIRD_PARTY.md) | Referências externas e direitos de terceiros |
| [Auditoria e validação](docs/AUDIT.md) | Preparação para publicação e verificações realizadas |

O código não implementa telemetria, autenticação ou envio a serviços externos. As opções usam o sistema de mods do jogo.

## Contribuir

Para reportar um problema, [abra uma issue](https://github.com/yGuilhermy/BaronShopCustomizer/issues) com a versão do jogo, versão do mod, filtros e multiplicadores utilizados e passos para reproduzir. Se anexar logs, remova informações pessoais.

Pull requests devem descrever a alteração e como ela foi validada. Para mudanças de lógica, execute a suíte; para comportamento da loja, informe se houve teste no headset.

## Licença

Código sob [MIT](LICENSE). Jogo, SDK e outros materiais de terceiros mantêm suas próprias licenças.
