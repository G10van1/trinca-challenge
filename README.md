# Desafio Técnico Trinca - API Churrasco

## Visão Geral

Este projeto é uma Web API para gerenciar convites, confirmações, e informações sobre um churrasco. 
Foi elaborado como solução para o desafio Trinca, pode ser visualizado na [wiki](https://github.com/trinca137/trinca-challenge/wiki/Comece-por-aqui) onde estão as informações relevantes sobre o desafio.
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

- **Implementação:** Sem informações.
- 
### 4. Churras? Quando?
- **Método:** GET
- **URL:** `http://localhost:7296/api/person/invites

- **Implementação:** Sem informações.
  
### 5. Aceitar convite
- **Método:** PUT
- **URL:** `http://localhost:7296/api/person/invites/69e13cc4-b5dd-4790-ba06-267b9205a6ff/accept

- **Implementação:** Sem informações.
  
### 6. Rejeitar convite
- **Método:** PUT
- **URL:** http://localhost:7296/api/person/invites/3d9702aa-6f1c-437c-a3ad-bd6c1daea143/decline

- **Implementação:** Sem informações.
  
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

- **Implementação:** Criado novo endpoint, calcula a lista de compras do churrsco com base em uma lista de convidados. O cálculo é realizado varrendo a lista e somando as quantidades de produtos conforme o status do convidado ('Accepted') e o tipo (isVeg), vegetariano ou não vegetariano. Somente moderadores podem acessar este endpoint. A lista de compras só é fornecida quando um churrasco está com estado confirmado.



