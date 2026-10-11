import { useState, useEffect } from "react"
import CrudTable from "../../../components/admin/common/CrudTable"
import ZoneDayForm from "./ZoneDayForm"
import type { ZoneDay } from "../../../models/ZoneDay"
import { getZoneDays, deleteZoneDay } from "../../../services/admin/zoneDayService"

type Props = {
  raceDayId: string
  onBack: () => void
}

type View = "LIST" | "CREATE" | "EDIT"

export default function ZoneDaysPage({ raceDayId, onBack }: Props) {
  const [zoneDays, setZoneDays] = useState<ZoneDay[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [view, setView] = useState<View>("LIST")
  const [selectedZoneDay, setSelectedZoneDay] = useState<ZoneDay | null>(null)

  useEffect(() => {
    fetchZoneDays()
  }, [])

  const fetchZoneDays = async () => {
    setIsLoading(true)
    try {
      const res = await getZoneDays(raceDayId)
      setZoneDays(res)
    } catch (err) {
      console.error(err)
    } finally {
      setIsLoading(false)
    }
  }

  const handleDelete = async (zoneDay: ZoneDay) => {
    if (!window.confirm("Delete this zone day?")) return

    setIsLoading(true)
    try {
      await deleteZoneDay(zoneDay.id)
      setZoneDays(prev => prev.filter(z => z.id !== zoneDay.id))
    } catch (err) {
      console.error(err)
      alert("Failed to delete zone day")
    } finally {
      setIsLoading(false)
    }
  }

  const handleSuccess = () => {
    setView("LIST")
    fetchZoneDays()
  }

  // --- FORM RENDER ---
  if (view === "CREATE") {
    return (
      <ZoneDayForm
        mode="create"
        raceDayId={raceDayId}
        onSuccess={handleSuccess}
      />
    )
  }

  if (view === "EDIT" && selectedZoneDay) {
    return (
      <ZoneDayForm
        mode="edit"
        initialData={selectedZoneDay}
        onSuccess={handleSuccess}
      />
    )
  }

  // --- TABLE RENDER ---
  return (
    <CrudTable<ZoneDay>
      title="Zone Days"
      data={zoneDays}
      isLoading={isLoading}
      columns={[
        { header: "Zone", render: (zd) => zd.zoneName },
        { header: "Seats Left", render: (zd) => zd.seatsLeft },
        { header: "Capacity", render: (zd) => zd.capacity },
        { header: "Price Addon", render: (zd) => `$${zd.priceAddon.toFixed(2)}` }
      ]}
      onCreate={() => setView("CREATE")}
      onEdit={(zd) => { setSelectedZoneDay(zd); setView("EDIT") }}
      onDelete={handleDelete}
      onBack={onBack}
    />
  )
}