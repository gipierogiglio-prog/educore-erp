import { useEffect, useState } from 'react'
import { api } from '@/api/client'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'

export default function Teachers() {
  const [teachers, setTeachers] = useState<any[]>([])
  useEffect(() => { api.academic.classes.list().then(() => {}); setTeachers([]) }, [])
  return (
    <div>
      <h1 className="text-2xl font-bold mb-6">Professores</h1>
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
            {teachers.map((t: any) => (
              <TableRow key={t.id}>
                <TableCell className="font-medium">{t.name}</TableCell>
                <TableCell>{t.email}</TableCell>
                <TableCell>{t.specialization || '-'}</TableCell>
                <TableCell>{t.subjectCount}</TableCell>
              </TableRow>
            ))}
            {teachers.length === 0 && (
              <TableRow><TableCell colSpan={4} className="text-center text-gray-400 py-8">Nenhum professor cadastrado</TableCell></TableRow>
            )}
          </TableBody>
        </Table>
      </Card>
    </div>
  )
}
