# EduCore ERP - Documentação da API (Backend)

## 📋 Visão Geral

Stack: .NET 8 + Entity Framework Core + PostgreSQL + JWT Auth

Autenticação: Bearer JWT
Base URL: `https://api.devgiglio.uk/api`

---

## 🔐 Autenticação

### POST /auth/login
```json
// Request
{ "email": "admin@escola.com", "password": "123456" }

// Response
{
  "token": "eyJhbG...",
  "name": "Administrador",
  "role": "org_admin",
  "expiresAt": "2026-05-22T18:41:16Z"
}
```

Todas as requisições (exceto login) precisam do header:
```
Authorization: Bearer <token>
```

---

## 🏫 Módulo Secretaria

### 1. Cursos (Courses)

Um **Curso** é a grade curricular que agrupa séries e disciplinas.
Ex: "Ensino Médio", "Ensino Fundamental I"

#### GET /courses
Lista todos os cursos da organização do usuário logado.
```json
// Response
[
  {
    "id": "guid",
    "name": "Ensino Médio",
    "description": "Curso regular do ensino médio",
    "durationYears": 3,
    "classCount": 6,
    "subjectCount": 12,
    "active": true,
    "createdAt": "2026-01-01T00:00:00Z"
  }
]
```

#### GET /courses/{id}
Detalhes de um curso (inclui turmas e disciplinas vinculadas).

#### POST /courses
```json
// Request
{
  "name": "Ensino Médio",
  "description": "Curso regular do ensino médio",
  "durationYears": 3
}

// Response: 201 Created
```

#### PUT /courses/{id}
```json
// Request (todos os campos opcionais)
{
  "name": "Ensino Fundamental II",
  "description": "6º ao 9º ano",
  "durationYears": 4
}
```

#### DELETE /courses/{id}
Desativa o curso (soft delete).

---

### 2. Disciplinas (Subjects)

#### GET /academic/subjects
Lista todas as disciplinas.

#### POST /academic/subjects
```json
{
  "name": "Matemática",
  "code": "MAT",
  "workload": 120
}
```

> Nota: Disciplinas podem ter `courseId` opcional para vincular ao curso.

---

### 3. Ano Letivo (SchoolYears)

#### GET /api/schoolyears
Lista anos letivos.

#### POST /api/schoolyears
```json
{
  "year": 2026,
  "startDate": "2026-02-01",
  "endDate": "2026-12-15",
  "description": "Ano Letivo 2026"
}
```

#### PATCH /api/schoolyears/{id}/status
```json
{ "status": "active" }
// Status possíveis: planned, active, completed, cancelled
```

---

### 4. Turmas (Classes)

#### GET /academic/classes
Lista turmas da organização.
```json
// Response
[
  {
    "id": "guid",
    "name": "3º Ano A",
    "shift": "morning",
    "year": 2026,
    "studentCount": 35,
    "courseId": "guid-opcional"
  }
]
```

#### POST /academic/classes
```json
{
  "name": "3º Ano A",
  "shift": "morning",
  "room": "Sala 101"
}
```
> Shift: "morning" | "afternoon" | "evening"

#### POST /academic/assign-teacher
Vincula um professor a uma disciplina em uma turma específica.
```json
{
  "teacherId": "guid",
  "subjectId": "guid",
  "classId": "guid"
}
```

---

### 5. Alunos (Students)

#### GET /students
Lista alunos.

#### GET /students/{id}
Detalhes do aluno.

#### POST /students
```json
{
  "name": "João Silva",
  "email": "joao@escola.com",
  "password": "senha123",
  "phone": "(11) 99999-8888",
  "classId": "guid",
  "guardianName": "Maria Silva",
  "guardianPhone": "(11) 98888-7777",
  "guardianRelationship": "mother"
}
```

#### PATCH /students/{id}/toggle-status
Ativa/desativa aluno.

#### PATCH /students/{id}/class
```json
{ "classId": "guid" }
// classId = null remove o aluno da turma
```

---

### 6. Funcionários / Professores (Teachers)

#### GET /teachers
Lista professores.

#### POST /teachers
```json
{
  "name": "Prof. Carlos",
  "email": "carlos@escola.com",
  "password": "senha123",
  "phone": "(11) 97777-6666",
  "specialization": "Matemática"
}
```

---

### 7. Matrículas (Enrollments)

#### GET /api/enrollments
Lista matrículas. Query params: `?year=2026`

#### GET /api/enrollments/student/{studentId}
Matrículas de um aluno específico.

#### POST /api/enrollments
```json
{
  "studentId": "guid",
  "classId": "guid",
  "schoolYear": 2026,
  "notes": "Aluno transferido da escola X"
}
```

#### PATCH /api/enrollments/{id}/status
```json
{ "status": "active" }
// Status: active, transferred, completed, cancelled
```

---

### 8. Notas (Grades)

#### GET /academic/grades/{classId}
Query params: `?bimester=1&year=2026`
Retorna as notas de todos os alunos de uma turma no bimestre.

#### POST /academic/grades/batch
Lançamento em lote de notas.
```json
{
  "subjectId": "guid",
  "classId": "guid",
  "bimester": 1,
  "year": 2026,
  "grades": [
    { "studentId": "guid", "value": 8.5, "recoveryValue": null }
  ]
}
```

---

### 9. Frequência (Attendance)

#### POST /academic/attendance/batch
Registro em lote de frequência.
```json
{
  "classId": "guid",
  "subjectId": "guid",
  "date": "2026-03-15",
  "attendances": [
    { "studentId": "guid", "present": true, "justification": null }
  ]
}
```

---

### 10. Dashboard

#### GET /api/dashboard
```json
{
  "totalStudents": 150,
  "totalTeachers": 18,
  "totalClasses": 8,
  "pendingInvoices": 12,
  "monthlyRevenue": 45000.00,
  "overdueAmount": 3200.00
}
```

---

## 🏢 Organização

### POST /api/organization
Criar empresa (requer super_admin).

### GET /api/organization/current
Dados da organização atual.

### PATCH /api/organization/current
Atualizar dados.

### PATCH /api/organization/{id}/status
Ativar/desativar empresa (super_admin).

---

## 👥 Usuários

### GET /api/users
Lista usuários da organização.

### POST /api/users
Criar usuário.

### PATCH /api/users/{id}/status
Ativar/desativar.

---

## 🔐 Permissões (RBAC)

Sistema completo de permissões por grupos e usuários.

### GET /api/permissions
Lista todas as permissões disponíveis.

### POST /api/permissions/groups
Criar grupo de permissão.

### POST /api/permissions/users/{userId}
Atribuir permissão a um usuário.

---

## 💰 Financeiro

### GET /api/financial/debtors
Inadimplentes.

### POST /api/financial/invoices/generate
Gerar mensalidades.

### POST /api/financial/payments
Registrar pagamento.

---

## 📝 Entidades do Banco

```sql
organizations     -- Empresas (escolas)
users             -- Usuários (super_admin | org_admin | teacher | student | guardian)
students          -- Alunos (vinculado a User)
teachers          -- Professores (vinculado a User)
guardians         -- Responsáveis (vinculado a User)
courses           -- Cursos (grade curricular) -- NOVO
classes           -- Turmas (opcionalmente vinculada a Course)
subjects          -- Disciplinas (opcionalmente vinculada a Course)
teacher_subjects  -- Vínculo Professor + Disciplina + Turma
school_years      -- Anos Letivos
enrollments       -- Matrículas
grades            -- Notas (bimestrais)
attendances       -- Frequência
invoices          -- Faturas/Mensalidades
tuition_plans     -- Planos de Mensalidade
permissions       -- Permissões do sistema
permission_groups -- Grupos de permissão
group_permissions -- Permissões do grupo
user_permissions  -- Permissões do usuário
user_groups       -- Grupos do usuário
```

## 🔄 Fluxo de Telas (sugestão para o Frontend)

```
Login → Dashboard
         ├── Secretaria
         │   ├── Cursos (CRUD)
         │   ├── Disciplinas (CRUD)
         │   ├── Ano Letivo (CRUD)
         │   ├── Turmas (CRUD + vincular professor)
         │   ├── Alunos (CRUD + matricular)
         │   ├── Funcionários (CRUD)
         │   └── Matrículas
         ├── Acadêmico
         │   ├── Lançar Notas
         │   ├── Lançar Frequência
         │   └── Boletim
         ├── Financeiro
         │   ├── Inadimplentes
         │   └── Mensalidades
         └── Admin
             ├── Permissões
             └── Configurações
```
