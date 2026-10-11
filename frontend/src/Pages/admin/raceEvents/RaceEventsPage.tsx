import { useState, useEffect } from "react"
import type { RaceEvent } from "../../../models/RaceEvent"
import { getAllRaceEvents, deleteRaceEvent } from "../../../services/admin/raceEventServiceAdmin"
import CrudTable from "../../../components/admin/common/CrudTable"
import RaceEventForm from "./RaceEventForm"

type Props = {
  onManageDays: (event: RaceEvent) => void
}

export default function RaceEventsPage({ onManageDays }: Props) {
  const [events, setEvents] = useState<RaceEvent[]>([])
  const [selectedEvent, setSelectedEvent] = useState<RaceEvent | null>(null)
  const [view, setView] = useState<"LIST" | "CREATE" | "EDIT">("LIST")
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    fetchEvents()
  }, [])

  const fetchEvents = async () => {
    setIsLoading(true)
    try {
      const res = await getAllRaceEvents()
      setEvents(res)
    } catch (err) {
      console.error(err)
    } finally {
      setIsLoading(false)
    }
  }

  const handleDelete = async (event: RaceEvent) => {
    if (!window.confirm("Delete this race event?")) return

    try {
      await deleteRaceEvent(event.id)
      setEvents(prev => prev.filter(ev => ev.id !== event.id))
    } catch (err) {
      console.error(err)
      alert("Failed to delete event")
    }
  }

  if (view === "CREATE") {
    return (
      <RaceEventForm
        mode="create"
        onSuccess={() => { setView("LIST"); fetchEvents() }}
      />
    )
  }

  if (view === "EDIT" && selectedEvent) {
    return (
      <RaceEventForm
        mode="edit"
        initialData={selectedEvent}
        onSuccess={() => { setView("LIST"); fetchEvents() }}
      />
    )
  }

  return (
    <CrudTable<RaceEvent>
      title="Race Events"
      data={events}
      isLoading={isLoading}
      columns={[
        { header: "Name", render: e => e.name },
        { header: "Start", render: e => new Date(e.startDate).toLocaleString() },
        { header: "End", render: e => new Date(e.endDate).toLocaleString() },
        { header: "Discount Until", render: e => new Date(e.discountValidUntil).toLocaleDateString() },
        { header: "Manage Days", render: e => (
            <button
              onClick={() => onManageDays(e)}
              className="bg-purple-500 text-white px-3 py-1 rounded"
            >
              Race Days
            </button>
          )}
      ]}
      onCreate={() => setView("CREATE")}
      onEdit={(e) => { setSelectedEvent(e); setView("EDIT") }}
      onDelete={handleDelete}
      // onBack je sada opcionalan pa ga ne moramo davati
    />
  )
}