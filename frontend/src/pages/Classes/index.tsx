import { useEffect, useState } from 'react'
import { api } from '@/api/client'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Plus } from 'lucide-react'

export default function Classes() {
  const [classes, setClasses] = useState<any[]>([])
  const [showForm, setShowForm] = useState(false)
  const [form, setForm] = useState({ name: '', shift: 'morning', room: '' })

  useEffect(() => { load() }, [])
  const load = () => api.academic.classes.list().then(setClasses)

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    await api.academic.classes.create(form)
    setShowForm(false)
    setForm({ name: '', shift: 'morning', room: '' })
    load()
  }

  const shiftLabel = (s: string) => ({ morning: 'Manhã', afternoon: 'Tarde', evening: 'Noite' } as any)[s] || s

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold">Turmas</h1>
        <Button onClick={() => setShowForm(!showForm)}><Plus className="w-4 h-4 mr-2" /> Nova Turma</Button>
      </div>

      {showForm && (
        <Card className="mb-6">
          <CardHeader><CardTitle>Nova Turma</CardTitle></CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-3 gap-4">
              <Input placeholder="Nome (ex: 3º Ano A)" value={form.name} onChange={e => setForm({ ...form, name: e.target.value })} required />
              <select className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 text-sm" value={form.shift} onChange={e => setForm({ ...form, shift: e.target.value })}>
                <option value="morning">Manhã</option>
                <option value="afternoon">Tarde</option>
                <option value="evening">Noite</option>
              </select>
              <Input placeholder="Sala" value={form.room} onChange={e => setForm({ ...form, room: e.target.value })} />
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
              <TableHead>Turma</TableHead>
              <TableHead>Turno</TableHead>
              <TableHead>Ano</TableHead>
              <TableHead>Sala</TableHead>
              <TableHead>Alunos</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {classes.map(c => (
              <TableRow key={c.id}>
                <TableCell className="font-medium">{c.name}</TableCell>
                <TableCell>{shiftLabel(c.shift)}</TableCell>
                <TableCell>{c.year}</TableCell>
                <TableCell>{c.room || '-'}</TableCell>
                <TableCell>{c.studentCount}</TableCell>
              </TableRow>
            ))}
            {classes.length === 0 && (
              <TableRow><TableCell colSpan={5} className="text-center text-gray-400 py-8">Nenhuma turma cadastrada</TableCell></TableRow>
            )}
          </TableBody>
        </Table>
      </Card>
    </div>
  )
}
