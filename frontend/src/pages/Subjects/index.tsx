import { useEffect, useState } from 'react'
import { api } from '@/api/client'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Plus } from 'lucide-react'

export default function Subjects() {
  const [subjects, setSubjects] = useState<any[]>([])
  const [showForm, setShowForm] = useState(false)
  const [form, setForm] = useState({ name: '', code: '', workload: 80 })

  useEffect(() => { load() }, [])
  const load = () => api.academic.subjects.list().then(setSubjects)

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    await api.academic.subjects.create(form)
    setShowForm(false)
    setForm({ name: '', code: '', workload: 80 })
    load()
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold">Disciplinas</h1>
        <Button onClick={() => setShowForm(!showForm)}><Plus className="w-4 h-4 mr-2" /> Nova Disciplina</Button>
      </div>

      {showForm && (
        <Card className="mb-6">
          <CardHeader><CardTitle>Nova Disciplina</CardTitle></CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-3 gap-4">
              <Input placeholder="Nome" value={form.name} onChange={e => setForm({ ...form, name: e.target.value })} required />
              <Input placeholder="Código (ex: MAT101)" value={form.code} onChange={e => setForm({ ...form, code: e.target.value })} required />
              <Input type="number" placeholder="Carga horária" value={form.workload} onChange={e => setForm({ ...form, workload: +e.target.value })} />
              <div className="flex gap-2">
                <Button type="submit">Salvar</Button>
                <Button variant="outline" onClick={() => setShowForm(false)}>Cancelar</Button>
              </div>
            </form>
          </CardContent>
        </Card>
      )}

      <Card>
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Código</TableHead>
              <TableHead>Nome</TableHead>
              <TableHead>Carga Horária</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {subjects.map(s => (
              <TableRow key={s.id}>
                <TableCell className="font-mono text-xs">{s.code}</TableCell>
                <TableCell className="font-medium">{s.name}</TableCell>
                <TableCell>{s.workload}h</TableCell>
              </TableRow>
            ))}
            {subjects.length === 0 && (
              <TableRow><TableCell colSpan={3} className="text-center text-gray-400 py-8">Nenhuma disciplina cadastrada</TableCell></TableRow>
            )}
          </TableBody>
        </Table>
      </Card>
    </div>
  )
}
