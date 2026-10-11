import React, { useEffect } from "react"
import type { ReactNode } from "react"
import { useNavigate } from "react-router-dom"
import { useAuth } from "../context/AuthContext"

interface Props {
  children: ReactNode
  role?: "ADMIN" | "USER"
}

export default function ProtectedRoute({ children, role }: Props): ReactNode {
  const { user } = useAuth()
  const navigate = useNavigate()

  // Rerender na promenu userm role i navigate
  useEffect(() => {
    if (!user) navigate("/login")
    else if (role && user.role !== role) navigate("/")
  }, [user, role, navigate])

  // Sprecava da se stranica ucita na delic sekunde ako ne sme
  if (!user || (role && user.role !== role)) return null

  // vracanje children-a. createElement zbog typescripta
  return React.createElement(React.Fragment, null, children)
}