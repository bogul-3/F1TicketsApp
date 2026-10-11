import { useEffect, useState } from "react"
import { getTicketsByRaceDay, type Ticket } from "../../../services/admin/reportingService"

type Props = {
  raceDayId: string
}

export default function RaceDayTicketsTable({ raceDayId }: Props) {

  const [tickets, setTickets] = useState<Ticket[]>([])

  useEffect(() => {
    fetchData()
  }, [raceDayId])

  const fetchData = async () => {
    const res = await getTicketsByRaceDay(raceDayId)
    setTickets(res)
  }

  return (
    <div className="flex flex-col gap-4">

      <h2 className="text-xl font-semibold">
        Tickets for Race Day
      </h2>

      <table className="border w-full">

        <thead className="bg-gray-100">
          <tr>
            <th className="p-2">Ticket</th>
            <th className="p-2">User</th>
            <th className="p-2">Quantity</th>
            <th className="p-2">Total Price</th>
            <th className="p-2">Purchased On</th>
          </tr>
        </thead>

        <tbody>
          {tickets.map(t => (
            <tr key={t.ticketId} className="border-t">
              <td className="p-2">{t.ticketId}</td>
              <td className="p-2">{t.userId}</td>
              <td className="p-2">{t.quantity}</td>
              <td className="p-2">${t.totalPrice}</td>
              <td className="p-2">
                {new Date(t.purchasedOn).toLocaleDateString()}
              </td>
            </tr>
          ))}
        </tbody>

      </table>
    </div>
  )
}