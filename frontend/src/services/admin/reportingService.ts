import axios from "axios"

const API_URL_REPORTING = "https://localhost:7138/api/Reporting"

export type SalesByDate = {
  date: string
  ticketsSold: number
}

export type Ticket = {
  ticketId: string
  userId: string
  zoneDayId: string
  raceDayId: string
  raceEventId: string
  quantity: number
  totalPrice: number
  purchasedOn: string
}

export const getSalesByDate = async (from?: string, to?: string) => {
  const params: Record<string, string> = {}
  if (from) params.from = from
  if (to) params.to = to

  const res = await axios.get(`${API_URL_REPORTING}/sales-by-date`, { params })
  return res.data as SalesByDate[]
}

export const getTicketsByRaceDay = async (raceDayId: string) => {
  const res = await axios.get(`${API_URL_REPORTING}/racedays/${raceDayId}/tickets`)
  return res.data as Ticket[]
}