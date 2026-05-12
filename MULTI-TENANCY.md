# Multi-Tenant Architecture — EduCore

## Modelo

```
Super Admin (nós)
├── Gerencia empresas (ativa/desativa)
├── Cria admin de cada empresa
└── Painel exclusivo

Company Admin (dono da escola)
├── Gerencia usuários da empresa
├── Acessa apenas dados da própria empresa
└── Cadastra professores, coordenadores, etc.

Usuários (professores, alunos, etc)
├── Apenas dados da empresa
└── Sem acesso entre empresas
```

## Entidades

```sql
organizations (id, name, slug, document, status, created_at)

users
  └── organization_id (nullable = super admin)
  └── role: super_admin | org_admin | teacher | student | guardian

students        ─┐
teachers        ─┤
classes         ─┤── organization_id (filtro obrigatório)
subjects        ─┤
grades          ─┤
attendance      ─┤
enrollments     ─┤
invoices        ─┘
```

## API

```
# Super Admin (role = super_admin)
POST   /api/admin/organizations         ← Criar empresa + admin
GET    /api/admin/organizations          ← Listar empresas
PATCH  /api/admin/organizations/:id      ← Ativar/desativar
GET    /api/organizations/current        ← Dados da empresa logada

# Empresa (filtrado por organization_id automaticamente)
GET    /api/students  ← só alunos da empresa do usuário logado
```
