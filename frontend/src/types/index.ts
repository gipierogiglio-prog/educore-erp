export interface User {
  id: string
  name: string
  email: string
  role: 'admin' | 'teacher' | 'student' | 'guardian'
}

export interface LoginResponse {
  token: string
  name: string
  role: string
  expiresAt: string
}

export interface Student {
  id: string
  name: string
  email: string
  enrollment: string
  className: string | null
  status: string
  guardianName: string | null
}

export interface Teacher {
  id: string
  name: string
  email: string
  specialization: string | null
  subjectCount: number
}

export interface Class {
  id: string
  name: string
  shift: string
  year: number
  studentCount: number
}

export interface Subject {
  id: string
  name: string
  code: string
  workload: number
}

export interface DashboardData {
  totalStudents: number
  totalTeachers: number
  totalClasses: number
  pendingInvoices: number
  monthlyRevenue: number
  overdueAmount: number
}
