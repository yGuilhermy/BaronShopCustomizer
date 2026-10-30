# Preparação para publicação — 29/10/2026

## Resultado

Repositório independente, licença MIT, fontes C# e testes públicos. Nenhum commit, staging ou push foi executado. Nenhum remoto está configurado.

## Três passadas de segurança

1. Segredos: nomes de arquivos sensíveis e padrões de credenciais verificados na lista pública. Nenhum achado. Não foram encontrados arquivos locais de credenciais protegidas para descriptografia ou busca cruzada.
2. Identidade e diretórios: verificados caminhos absolutos, referências a projetos privados, identidade do autor, e-mails e IPs privados. Nenhum achado nos arquivos públicos. A autoria pública do manifesto usa o nome coletivo do projeto. O build recebe referências por `SdkDir`, `MOD_SDK_DIR` ou pasta relativa `sdk/`.
3. Artefatos e imagens: lista conferida com `git ls-files --others --exclude-standard`. DLLs, PDBs, builds e ferramentas locais ficam fora dela. Não havia capturas de tela. O único visual público é um diagrama SVG criado para a documentação, com texto funcional sem dados privados.

Os resultados descrevem os arquivos preparados nesta data; não constituem garantia sobre alterações futuras.

## Triagem

| Mantido | Motivo |
| --- | --- |
| `src/` | Cópia dos fontes correspondentes ao mod |
| `tests/` | Verifica seleção de estoque sem jogo instalado |
| Manifesto, projeto e build | Metadados e compilação independente |
| Documentação, diagrama e licença | Uso, limites técnicos e permissões |

| Ignorado / não incorporado | Motivo |
| --- | --- |
| DLL e PDB originais | Compilados e símbolos locais; preservados no disco |
| `bin/`, `obj/`, `dist/` | Gerados pelo build |
| Referências do jogo e stubs | Não redistribuir SDK; não produzir release com APIs simuladas |
| Logs, scratch, backups e planos de IA | Não são necessários ao clone público |
| Credenciais e bancos locais | Não pertencem ao repositório |

Os projetos originais e backups externos a esta pasta foram preservados.

## Validação

Build Release sem erros, com 7 verificações públicas aprovadas. Dois avisos MSB3277 vêm das referências reais de ThunderRoad: conflitos de versão de `System.Net.Http` e `System.IO.Compression`. Os avisos permanecem visíveis e exigem revisão com um conjunto de referências compatível com o destino antes de afirmar compatibilidade completa.

O build e os testes também passaram em uma cópia isolada contendo apenas os arquivos públicos, sem binários originais, stubs ou arquivos ignorados. As referências externas foram fornecidas explicitamente por parâmetro.

A validação de compilação usa referências PC disponíveis. Execução Unity/IL2CPP, compatibilidade com Nomad e comportamento em VR precisam ser conferidos no headset. O README remove alegações anteriores de tempo de atualização garantido.
