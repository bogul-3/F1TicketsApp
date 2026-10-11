import { useEffect, useState } from "react"
import { getSalesByDate, type SalesByDate } from "../../../services/admin/reportingService"
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer
} from "recharts"

export default function SalesByDateChart() {
  const [data, setData] = useState<SalesByDate[]>([])
  const [startDate, setStartDate] = useState<string>("")
  const [endDate, setEndDate] = useState<string>("")

  useEffect(() => {
    fetchData()
  }, [])

  const fetchData = async () => {
    const res = await getSalesByDate(
      startDate ? `${startDate}T00:00:00Z` : undefined, 
      endDate ? `${endDate}T23:59:59Z` : undefined
    )
    setData(res)
  }

  useEffect(() => {
    fetchData()
  }, [startDate, endDate])

  return (
    <div className="w-full h-[400px]">
      <h2 className="text-xl font-semibold mb-4">Tickets Sold by Purchase Date</h2>

      {/* DATE PICKERS */}
      <div className="flex gap-4 mb-4">
        <div>
          <label className="mr-2 font-semibold">Start Date:</label>
          <input
            type="date"
            value={startDate}
            onChange={(e) => setStartDate(e.target.value)}
            className="border rounded px-2 py-1"
          />
        </div>
        <div>
          <label className="mr-2 font-semibold">End Date:</label>
          <input
            type="date"
            value={endDate}
            onChange={(e) => setEndDate(e.target.value)}
            className="border rounded px-2 py-1"
          />
        </div>
      </div>

      <ResponsiveContainer width="100%" height="100%">
        <LineChart data={data}>
          <CartesianGrid strokeDasharray="3 3" />
          <XAxis dataKey="date" />
          <YAxis />
          <Tooltip />
          <Line type="monotone" dataKey="ticketsSold" stroke="#8884d8" />
        </LineChart>
      </ResponsiveContainer>
    </div>
  )
}