import { useEffect, useState } from 'react'
import { api } from '@/api/client'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { Plus, Search } from 'lucide-react'

export default function Teachers() {
  const [teachers, setTeachers] = useState<any[]>([])
  const [search, setSearch] = useState('')
  const [showForm, setShowForm] = useState(false)
  const [form, setForm] = useState({ name: '', email: '', password: '123456', phone: '', specialization: '' })

  useEffect(() => { load() }, [])
  const load = () => api.teachers.list().then(setTeachers)

  const filtered = teachers.filter(t =>
    t.name.toLowerCase().includes(search.toLowerCase()) ||
    t.email.toLowerCase().includes(search.toLowerCase())
  )

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    try {
      const payload = Object.fromEntries(
        Object.entries(form).filter(([_, v]) => v !== '')
      )
      await api.teachers.create(payload)
      setShowForm(false)
      setForm({ name: '', email: '', password: '123456', phone: '', specialization: '' })
      load()
    } catch (err: any) {
      alert(err.message || 'Erro ao salvar professor')
    }
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold">Professores</h1>
        <Button onClick={() => setShowForm(!showForm)}>
          <Plus className="w-4 h-4 mr-2" /> Novo Professor
        </Button>
      </div>

      {showForm && (
        <Card className="mb-6">
          <CardHeader><CardTitle>Novo Professor</CardTitle></CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <Input placeholder="Nome completo" value={form.name}
                onChange={e => setForm({ ...form, name: e.target.value })} required />
              <Input type="email" placeholder="Email" value={form.email}
                onChange={e => setForm({ ...form, email: e.target.value })} required />
              <Input placeholder="Telefone" value={form.phone}
                onChange={e => setForm({ ...form, phone: e.target.value })} />
              <Input placeholder="Especialização" value={form.specialization}
                onChange={e => setForm({ ...form, specialization: e.target.value })} />
              <Input placeholder="Senha" value={form.password}
                onChange={e => setForm({ ...form, password: e.target.value })} />
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
        <Input className="pl-9" placeholder="Buscar por nome ou email..." value={search}
          onChange={e => setSearch(e.target.value)} />
      </div>

      <Card>
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Nome</TableHead>
              <TableHead>Email</TableHead>
              <TableHead>Especialização</TableHead>
              <TableHead>Disciplinas</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {filtered.map((t: any) => (
              <TableRow key={t.id}>
                <TableCell className="font-medium">{t.name}</TableCell>
                <TableCell>{t.email}</TableCell>
                <TableCell>{t.specialization || '-'}</TableCell>
                <TableCell>{t.subjectCount}</TableCell>
              </TableRow>
            ))}
            {filtered.length === 0 && (
              <TableRow><TableCell colSpan={4} className="text-center text-gray-400 py-8">Nenhum professor cadastrado</TableCell></TableRow>
            )}
          </TableBody>
        </Table>
      </Card>
    </div>
  )
}
