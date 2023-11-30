# Desafio Técnico Trinca - API Churrasco

## Visão Geral

Este projeto é uma Web API para gerenciar convites, confirmações, e informações sobre um churrasco. 
Foi elaborado como solução para o desafio Trinca. Pode ser visualizado na [wiki](https://github.com/trinca137/trinca-challenge/wiki/Comece-por-aqui) onde estão as informações relevantes sobre o desafio.
Abaixo estão detalhes sobre cada solução implementada em cada endpoint disponível.

## Endpoints

### 1. **Alguém pilha?**
- **Método:** POST
- **URL:** `http://localhost:7296/api/churras`
  
- **Implementação:** Foi refatorado o código para que a interface "IEventStore" ficasse encapsulada no assembly de origem Domain.
  
### 2. E aí, vai rolar?
- **Método:** GET
- **URL:** `http://localhost:7296/api/churras
  
- **Implementação:** Correção de bug para não mostrar churrascos rejeitados por um moderador.
  
### 3. Tem aval dos sócios?
- **Método:** PUT
- **URL:** `http://localhost:7296/api/churras/{{churras-id}}/moderar

- **Implementação:** Os bugs conhecidos foram todos solucionados, abaixo as correções e melhorias realizadas:
- 
              ✔️ Moderadores recebem convite apenas na criação do churrasco, na aprovação apenas os demais funcionários recebem convite;
  
              ✔️ Convites são enviados para os funcionários apenas quando o moderador informa que o churrasco vai acontecer;
  
              ✔️ Quando a proposta de churrasco é rejeitada, os convites pendentes associados são todos rejeitados;
  
              ✔️ O churrasco uma vez rejeitado pelo moderador, não pode mais ser aprovado. Depois de aprovado, pode ser rejeitado.
  
### 4. Churras? Quando?
- **Método:** GET
- **URL:** `http://localhost:7296/api/person/invites

- **Implementação:** Aqui foram realizadas apenas melhorias e organização de código.
  
### 5. Aceitar convite
- **Método:** PUT
- **URL:** `http://localhost:7296/api/person/invites/69e13cc4-b5dd-4790-ba06-267b9205a6ff/accept

- **Implementação:** Implementado tratamento para cálculo da lista de compras. Foi implementado uma lista de convidados que pertence a um churrasco. A pessoa é inserida nesta lista no momento do aceite ou rejeição do convite, sendo que a informação se é vegetariano é atualizada no momento de aceite do convite.
            A informação de status é atualizada para 'Accepted' neste caso.
  
### 6. Rejeitar convite
- **Método:** PUT
- **URL:** http://localhost:7296/api/person/invites/3d9702aa-6f1c-437c-a3ad-bd6c1daea143/decline

- **Implementação:** Da mesma forma que o aceito do convite, a rejeição também entra na lista de convidados do churrasco. Neste caso o status é atualizado para 'Declined'. O tipo de pessoa, vegetaria ou não neste caso não é relevante pois não vai ser contabilizada na lista de compras.
  
### 7. O que comprar?
- **Método:** Método: GET
- **URL:** `URL: http://localhost:7296/api/churras/{{churras-id}}/shopping
- **Cabeçalho:** 
  - personId: {{moderador-1}}
- **Corpo da Requisição:** Vazio
- **Resposta:** 
```json
{
    "QtyKgMeat": "2,10",
    "QtyKgVegetables": "2,70"
}
```

- **Implementação:** Criado novo endpoint, este calcula a lista de compras do churrasco com base em uma lista de convidados. O cálculo é realizado varrendo esta lista e somando as quantidades de produtos conforme o status do convidado ('Accepted') e o tipo (isVeg), vegetariano ou não vegetariano. Somente moderadores podem acessar este endpoint. A lista de compras só é fornecida quando um churrasco está com estado confirmado.

### Demais considerações:

Foram refatoradas todas as lógicas da API e encapsuladas no projeto Domain. Agora podem ser acessadas pela API através de serviços do Domain. 



