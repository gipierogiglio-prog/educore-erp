import { useEffect, useState } from 'react'
import { api } from '@/api/client'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { Plus, Search, ToggleLeft, ToggleRight } from 'lucide-react'

export default function Students() {
  const [students, setStudents] = useState<any[]>([])
  const [classes, setClasses] = useState<any[]>([])
  const [search, setSearch] = useState('')
  const [showForm, setShowForm] = useState(false)
  const [editClass, setEditClass] = useState<{ id: string; classId: string } | null>(null)
  const [form, setForm] = useState({ name: '', email: '', password: '123456', phone: '', classId: '' })

  useEffect(() => { load(); api.academic.classes.list().then(setClasses) }, [])
  const load = () => api.students.list().then(setStudents)

  const filtered = students.filter(s =>
    s.name.toLowerCase().includes(search.toLowerCase()) ||
    s.enrollment.toLowerCase().includes(search.toLowerCase())
  )

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    try {
      const payload = Object.fromEntries(
        Object.entries(form).filter(([_, v]) => v !== '')
      )
      await api.students.create(payload)
      setShowForm(false)
      setForm({ name: '', email: '', password: '123456', phone: '', classId: '' })
      load()
    } catch (err: any) {
      alert(err.message || 'Erro ao salvar aluno')
    }
  }

  const handleChangeClass = async (studentId: string) => {
    if (!editClass) return
    try {
      await fetch(`/api/students/${studentId}/class`, {
        method: 'PATCH',
        headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${localStorage.getItem('token')}` },
        body: JSON.stringify({ classId: editClass.classId || null })
      })
      setEditClass(null)
      load()
    } catch (err: any) {
      alert(err.message)
    }
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold">Alunos</h1>
        <Button onClick={() => setShowForm(!showForm)}>
          <Plus className="w-4 h-4 mr-2" /> Novo Aluno
        </Button>
      </div>

      {showForm && (
        <Card className="mb-6">
          <CardHeader><CardTitle>Novo Aluno</CardTitle></CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <Input placeholder="Nome completo" value={form.name} onChange={e => setForm({ ...form, name: e.target.value })} required />
              <Input type="email" placeholder="Email" value={form.email} onChange={e => setForm({ ...form, email: e.target.value })} required />
              <Input placeholder="Telefone" value={form.phone} onChange={e => setForm({ ...form, phone: e.target.value })} />
              <Input placeholder="Senha" value={form.password} onChange={e => setForm({ ...form, password: e.target.value })} />
              <select className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 text-sm"
                value={form.classId} onChange={e => setForm({ ...form, classId: e.target.value })}>
                <option value="">Sem turma</option>
                {classes.map(c => <option key={c.id} value={c.id}>{c.name} - {c.shift === 'morning' ? 'Manhã' : c.shift === 'afternoon' ? 'Tarde' : 'Noite'}</option>)}
              </select>
              <div className="md:col-span-2 flex gap-2">
                <Button type="submit">Salvar</Button>
                <Button variant="outline" onClick={() => setShowForm(false)}>Cancelar</Button>
              </div>
            </form>
          </CardContent>
        </Card>
      )}

      <div className="relative mb-4">
        <Search className="absolute left-3 top-2.5 w-4 h-4 text-gray-400" />
        <Input className="pl-9" placeholder="Buscar por nome ou matrícula..." value={search} onChange={e => setSearch(e.target.value)} />
      </div>

      <Card>
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Matrícula</TableHead>
              <TableHead>Nome</TableHead>
              <TableHead>Turma</TableHead>
              <TableHead>Status</TableHead>
              <TableHead className="text-right">Ações</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {filtered.map(s => (
              <TableRow key={s.id}>
                <TableCell className="font-mono text-xs">{s.enrollment}</TableCell>
                <TableCell className="font-medium">{s.name}</TableCell>
                <TableCell>
                  {editClass?.id === s.id ? (
                    <div className="flex gap-1 items-center">
                      <select className="flex h-8 rounded-md border border-input bg-transparent px-2 text-xs"
                        value={editClass.classId}
                        onChange={e => setEditClass({ ...editClass, classId: e.target.value })}>
                        <option value="">Sem turma</option>
                        {classes.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
                      </select>
                      <Button size="sm" variant="default" onClick={() => handleChangeClass(s.id)}>OK</Button>
                      <Button size="sm" variant="ghost" onClick={() => setEditClass(null)}>✕</Button>
                    </div>
                  ) : (
                    <span onClick={() => setEditClass({ id: s.id, classId: s.classId || '' })}
                      className="cursor-pointer hover:text-primary text-sm">
                      {s.className || <span className="text-gray-400 italic">Sem turma</span>}
                    </span>
                  )}
                </TableCell>
                <TableCell>
                  <span className={`px-2 py-1 rounded-full text-xs font-medium ${s.status === 'Ativo' ? 'bg-green-100 text-green-700' : 'bg-gray-100 text-gray-500'}`}>
                    {s.status}
                  </span>
                </TableCell>
                <TableCell className="text-right">
                  <Button variant="ghost" size="sm" onClick={() => api.students.toggleStatus(s.id).then(load)}>
                    {s.status === 'Ativo' ? <ToggleRight className="w-4 h-4 text-green-500" /> : <ToggleLeft className="w-4 h-4 text-gray-400" />}
                  </Button>
                </TableCell>
              </TableRow>
            ))}
            {filtered.length === 0 && (
              <TableRow><TableCell colSpan={5} className="text-center text-gray-400 py-8">Nenhum aluno encontrado</TableCell></TableRow>
            )}
          </TableBody>
        </Table>
      </Card>
    </div>
  )
}
