import { useEffect, useState } from 'react'
import { apiFetch } from '@/api/client'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { Plus } from 'lucide-react'

export default function SchoolYears() {
  const [years, setYears] = useState<any[]>([])
  const [showForm, setShowForm] = useState(false)
  const [form, setForm] = useState({ year: new Date().getFullYear(), startDate: '', endDate: '', description: '' })

  useEffect(() => { load() }, [])
  const load = () => apiFetch<any[]>('/schoolYears').then(setYears).catch(() => {})

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    try {
      await apiFetch('/schoolYears', {
        method: 'POST',
        body: JSON.stringify({ ...form, year: Number(form.year) }),
      })
      setShowForm(false)
      setForm({ year: new Date().getFullYear(), startDate: '', endDate: '', description: '' })
      load()
    } catch (err: any) { alert(err.message || 'Erro ao salvar') }
  }

  const statusColors: Record<string, string> = {
    planned: 'bg-gray-100 text-gray-600',
    active: 'bg-green-100 text-green-700',
    completed: 'bg-blue-100 text-blue-700',
    cancelled: 'bg-red-100 text-red-700',
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold">Anos Letivos</h1>
        <Button onClick={() => setShowForm(!showForm)}>
          <Plus className="w-4 h-4 mr-2" /> Novo Ano Letivo
        </Button>
      </div>

      {showForm && (
        <Card className="mb-6">
          <CardHeader><CardTitle>Novo Ano Letivo</CardTitle></CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <Input type="number" placeholder="Ano" value={form.year}
                onChange={e => setForm({ ...form, year: +e.target.value })} required />
              <Input type="date" placeholder="Início" value={form.startDate}
                onChange={e => setForm({ ...form, startDate: e.target.value })} required />
              <Input type="date" placeholder="Fim" value={form.endDate}
                onChange={e => setForm({ ...form, endDate: e.target.value })} required />
              <Input placeholder="Descrição (ex: 2026 - Ensino Fundamental)"
                value={form.description} onChange={e => setForm({ ...form, description: e.target.value })} />
              <div className="md:col-span-2 flex gap-2">
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
              <TableHead>Ano</TableHead>
              <TableHead>Descrição</TableHead>
              <TableHead>Início</TableHead>
              <TableHead>Término</TableHead>
              <TableHead>Status</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {years.map((y: any) => (
              <TableRow key={y.id}>
                <TableCell className="font-bold">{y.year}</TableCell>
                <TableCell>{y.description || '-'}</TableCell>
                <TableCell>{new Date(y.startDate).toLocaleDateString('pt-BR')}</TableCell>
                <TableCell>{new Date(y.endDate).toLocaleDateString('pt-BR')}</TableCell>
                <TableCell>
                  <span className={`px-2 py-1 rounded-full text-xs font-medium ${statusColors[y.status] || ''}`}>
                    {{ planned: 'Planejado', active: 'Ativo', completed: 'Concluído', cancelled: 'Cancelado' }[y.status] || y.status}
                  </span>
                </TableCell>
              </TableRow>
            ))}
            {years.length === 0 && (
              <TableRow><TableCell colSpan={5} className="text-center text-gray-400 py-8">Nenhum ano letivo cadastrado</TableCell></TableRow>
            )}
          </TableBody>
        </Table>
      </Card>
    </div>
  )
}
