# Requisitos Legais - Sistema de Ponto Eletrônico (REP-P)

## Portaria 671/2021 - Ministério do Trabalho

### Requisitos Técnicos Obrigatórios

| Requisito | Descrição | Status |
|-----------|-----------|--------|
| **Registro no INPI** | O software REP-P deve ser registrado no Instituto Nacional da Propriedade Industrial | ⬜ Pendente |
| **Atestado Técnico e Termo de Responsabilidade** | Documento assinado digitalmente pelo responsável técnico e legal da empresa desenvolvedora | ⬜ Pendente |
| **Certificado Digital** | Comprovantes de registro devem ser assinados com certificado digital da empresa desenvolvedora | ⬜ Pendente |
| **Conexão com Internet** | Sistema deve estar conectado para transmissão em tempo real dos dados | ⬜ Implementar |
| **Geração de AFD** | Arquivo Fonte de Dados conforme layout do Anexo V da portaria | ⬜ Implementar |
| **Geração de AEJ** | Arquivo Eletrônico de Jornada em formato XML | ⬜ Implementar |
| **Espelho de Ponto** | Relatório mensal disponibilizado ao trabalhador | ⬜ Implementar |
| **Comprovante de Registro** | Emitir comprovante com CPF, data/hora, identificação do empregador, assinatura eletrônica | ⬜ Implementar |

### Requisitos de Autenticação

| Método | Obrigatório | Notas |
|--------|-------------|-------|
| Senha | Sim | Mínimo suportado |
| Biometria (digital) | Opcional | Recomendado |
| Reconhecimento Facial | Opcional | Recomendado |
| Geolocalização | Opcional | Para trabalho remoto |

### Requisitos de Segurança

| Requisito | Especificação |
|-----------|---------------|
| **Criptografia de dados** | AES-256 para armazenamento e transmissão |
| **Integridade dos registros** | Registros não podem ser alterados após gravação |
| **Rastreabilidade** | Log de todas as edições com usuário, data/hora e motivo |
| **Armazenamento** | Mínimo 5 anos de retenção dos dados |
| **Disponibilidade** | Infraestrutura em nuvem com redundância |

### Arquivos Fiscais

#### AFD - Arquivo Fonte de Dados
- Formato: Texto com layout específico
- Campos: NSR, tipo de marcação, data/hora, PIS
- Deve ser exportável para fiscalização
- Assinatura eletrônica obrigatória

#### AEJ - Arquivo Eletrônico de Jornada
- Formato: XML estruturado
- Substitui o antigo ACJEF
- Contém detalhamento de ocorrências (horas extras, ajustes, faltas)
- Gerado pelo Programa de Tratamento de Ponto

---

## LGPD - Lei Geral de Proteção de Dados (Lei 13.709/2018)

### Dados Pessoais Tratados

| Tipo | Dados | Classificação |
|------|-------|---------------|
| Identificação | Nome, CPF, PIS | Dado Pessoal |
| Contato | Email, telefone | Dado Pessoal |
| Biométrico | Digital, face | **Dado Sensível** |
| Localização | Latitude, longitude | Dado Pessoal |
| Jornada | Horários de entrada/saída | Dado Pessoal |

### Requisitos de Conformidade

| Requisito | Implementação |
|-----------|---------------|
| **Base Legal** | Execução de contrato de trabalho (Art. 7º, V) |
| **Finalidade** | Controle de jornada conforme CLT |
| **Consentimento para biometria** | Obrigatório por ser dado sensível (Art. 11) |
| **Direito de acesso** | Endpoint para titular consultar seus dados |
| **Direito de portabilidade** | Exportação dos dados em formato estruturado |
| **Direito de exclusão** | Após período legal de retenção (5 anos) |
| **Minimização** | Coletar apenas dados necessários |
| **Segurança** | Medidas técnicas e administrativas |

### Incidentes de Segurança

- Comunicar à ANPD em prazo razoável
- Notificar titulares afetados
- Documentar ocorrência e medidas tomadas

### Documentação Necessária

| Documento | Descrição |
|-----------|-----------|
| **Política de Privacidade** | Informar tratamento de dados aos usuários |
| **Termo de Consentimento** | Para coleta de dados biométricos |
| **RIPD** | Relatório de Impacto à Proteção de Dados (para biometria) |
| **Registro de Operações** | Documentar todas as operações de tratamento |

---

## Integração com eSocial

| Evento | Descrição | Prazo |
|--------|-----------|-------|
| S-2200 | Cadastramento inicial do vínculo | Admissão |
| S-2206 | Alteração de contrato de trabalho | Até dia 15 do mês seguinte |
| S-2299 | Desligamento | Até 10 dias |
| S-1200 | Remuneração | Mensal |

---

## Checklist de Implementação

### Fase 1 - MVP
- [ ] Registro de marcações (entrada/saída)
- [ ] Autenticação por senha
- [ ] Comprovante digital de registro
- [ ] Espelho de ponto básico
- [ ] Criptografia de dados em trânsito (HTTPS)
- [ ] Criptografia de senhas (bcrypt/Argon2)

### Fase 2 - Compliance
- [ ] Geração de AFD
- [ ] Geração de AEJ
- [ ] Registro no INPI
- [ ] Atestado Técnico
- [ ] Certificado digital para assinaturas
- [ ] Biometria/Reconhecimento facial
- [ ] Geolocalização
- [ ] Integração eSocial

### Fase 3 - LGPD Completo
- [ ] Política de Privacidade
- [ ] Termo de Consentimento para biometria
- [ ] Portal do titular (acesso aos dados)
- [ ] Exportação de dados (portabilidade)
- [ ] Processo de exclusão de dados
- [ ] RIPD documentado
- [ ] Procedimento de incidentes

---

## Referências

- [Portaria 671/2021 - MTP](https://www.gov.br/trabalho-e-emprego/pt-br/assuntos/legislacao/portarias-1/portarias-vigentes-3)
- [Perguntas e Respostas - Portaria 671](https://www.gov.br/trabalho-e-emprego/pt-br/assuntos/inspecao-do-trabalho/fiscalizacao-do-trabalho/Perguntas%20e%20Respostas%20REP)
- [LGPD - Lei 13.709/2018](https://www.planalto.gov.br/ccivil_03/_ato2015-2018/2018/lei/l13709.htm)
- [Guia LGPD - Governo Digital](https://www.gov.br/governodigital/pt-br/seguranca-e-protecao-de-dados/guias-operacionais-para-adequacao-a-lei-geral-de-protecao-de-dados-pessoais-lgpd)

---

*Documento gerado em: Novembro/2025*
*Última atualização da Portaria 671: Portarias 1.486, 3.717 e 4.198 de 2022*