import { useEffect, useState } from 'react'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Users, GraduationCap, School, DollarSign, AlertCircle, TrendingUp } from 'lucide-react'
import { api } from '@/api/client'

export default function Dashboard() {
  const [data, setData] = useState({
    totalStudents: 0, totalTeachers: 0, totalClasses: 0,
    pendingInvoices: 0, monthlyRevenue: 0, overdueAmount: 0,
  })

  useEffect(() => {
    Promise.all([
      api.students.list().then(r => setData(d => ({ ...d, totalStudents: r.length }))).catch(() => {}),
      api.academic.classes.list().then(r => setData(d => ({ ...d, totalClasses: r.length }))).catch(() => {}),
      api.academic.subjects.list().then(r => setData(d => ({ ...d, totalTeachers: r.length }))).catch(() => {}),
    ])
  }, [])

  const cards = [
    { title: 'Alunos', value: data.totalStudents, icon: Users, color: 'bg-blue-500' },
    { title: 'Professores', value: data.totalTeachers, icon: GraduationCap, color: 'bg-green-500' },
    { title: 'Turmas', value: data.totalClasses, icon: School, color: 'bg-purple-500' },
    { title: 'Mensalidades Pendentes', value: data.pendingInvoices, icon: DollarSign, color: 'bg-amber-500' },
    { title: 'Faturamento Mensal', value: `R$ ${data.monthlyRevenue.toFixed(0)}`, icon: TrendingUp, color: 'bg-emerald-500' },
    { title: 'Inadimplência', value: `R$ ${data.overdueAmount.toFixed(0)}`, icon: AlertCircle, color: 'bg-red-500' },
  ]

  return (
    <div>
      <h1 className="text-2xl font-bold mb-6">Dashboard</h1>
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {cards.map(card => (
          <Card key={card.title}>
            <CardHeader className="flex flex-row items-center justify-between pb-2">
              <CardTitle className="text-sm font-medium text-gray-500">{card.title}</CardTitle>
              <div className={`${card.color} p-2 rounded-lg`}>
                <card.icon className="w-4 h-4 text-white" />
              </div>
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{card.value}</div>
            </CardContent>
          </Card>
        ))}
      </div>
    </div>
  )
}
