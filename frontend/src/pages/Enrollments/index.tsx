import { useEffect, useState } from 'react'
import { api, apiFetch } from '@/api/client'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { Plus, Search } from 'lucide-react'

export default function Enrollments() {
  const [enrollments, setEnrollments] = useState<any[]>([])
  const [students, setStudents] = useState<any[]>([])
  const [classes, setClasses] = useState<any[]>([])
  const [search, setSearch] = useState('')
  const [showForm, setShowForm] = useState(false)
  const [form, setForm] = useState({ studentId: '', classId: '', schoolYear: new Date().getFullYear(), notes: '' })
  const [yearFilter, setYearFilter] = useState(new Date().getFullYear())

  useEffect(() => {
    load()
    api.students.list().then(setStudents)
    api.academic.classes.list().then(setClasses)
  }, [yearFilter])

  const load = () => {
    apiFetch(`/enrollments?year=${yearFilter}`).then(setEnrollments).catch(() => {})
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    try {
      const payload = { ...form, schoolYear: Number(form.schoolYear) }
      if (!payload.studentId || !payload.classId) { alert('Selecione aluno e turma'); return }
      await apiFetch('/enrollments', { method: 'POST', body: JSON.stringify(payload) })
      setShowForm(false)
      setForm({ studentId: '', classId: '', schoolYear: new Date().getFullYear(), notes: '' })
      load()
    } catch (err: any) { alert(err.message || 'Erro ao matricular') }
  }

  const statusColors: Record<string, string> = {
    active: 'bg-green-100 text-green-700',
    transferred: 'bg-blue-100 text-blue-700',
    completed: 'bg-gray-100 text-gray-600',
    cancelled: 'bg-red-100 text-red-700',
  }

  const statusLabels: Record<string, string> = {
    active: 'Ativa', transferred: 'Transferido', completed: 'Concluído', cancelled: 'Cancelado',
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold">Matrículas</h1>
        <Button onClick={() => setShowForm(!showForm)}>
          <Plus className="w-4 h-4 mr-2" /> Nova Matrícula
        </Button>
      </div>

      {/* Year filter */}
      <div className="flex gap-4 mb-4 items-center">
        <label className="text-sm font-medium">Ano:</label>
        <select className="flex h-9 rounded-md border border-input bg-transparent px-3 text-sm"
          value={yearFilter} onChange={e => setYearFilter(Number(e.target.value))}>
          {[2024, 2025, 2026, 2027].map(y => <option key={y} value={y}>{y}</option>)}
        </select>
      </div>

      {showForm && (
        <Card className="mb-6">
          <CardHeader><CardTitle>Nova Matrícula</CardTitle></CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="text-sm font-medium mb-1 block">Aluno</label>
                <select className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 text-sm"
                  value={form.studentId} onChange={e => setForm({ ...form, studentId: e.target.value })} required>
                  <option value="">Selecione...</option>
                  {students.map(s => <option key={s.id} value={s.id}>{s.name} ({s.enrollment})</option>)}
                </select>
              </div>
              <div>
                <label className="text-sm font-medium mb-1 block">Turma</label>
                <select className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 text-sm"
                  value={form.classId} onChange={e => setForm({ ...form, classId: e.target.value })} required>
                  <option value="">Selecione...</option>
                  {classes.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
                </select>
              </div>
              <Input type="number" placeholder="Ano" value={form.schoolYear}
                onChange={e => setForm({ ...form, schoolYear: +e.target.value })} />
              <Input placeholder="Observações" value={form.notes}
                onChange={e => setForm({ ...form, notes: e.target.value })} />
              <div className="md:col-span-2 flex gap-2">
                <Button type="submit">Matricular</Button>
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
              <TableHead>Aluno</TableHead>
              <TableHead>Matrícula</TableHead>
              <TableHead>Turma</TableHead>
              <TableHead>Ano</TableHead>
              <TableHead>Status</TableHead>
              <TableHead>Data</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {enrollments.filter((e: any) =>
              e.studentName.toLowerCase().includes(search.toLowerCase())
            ).map((e: any) => (
              <TableRow key={e.id}>
                <TableCell className="font-medium">{e.studentName}</TableCell>
                <TableCell className="font-mono text-xs">{e.studentEnrollment}</TableCell>
                <TableCell>{e.className}</TableCell>
                <TableCell>{e.schoolYear}</TableCell>
                <TableCell>
                  <span className={`px-2 py-1 rounded-full text-xs font-medium ${statusColors[e.status] || 'bg-gray-100'}`}>
                    {statusLabels[e.status] || e.status}
                  </span>
                </TableCell>
                <TableCell className="text-sm text-gray-500">
                  {new Date(e.enrollmentDate).toLocaleDateString('pt-BR')}
                </TableCell>
              </TableRow>
            ))}
            {enrollments.length === 0 && (
              <TableRow><TableCell colSpan={6} className="text-center text-gray-400 py-8">Nenhuma matrícula encontrada</TableCell></TableRow>
            )}
          </TableBody>
        </Table>
      </Card>
    </div>
  )
}
