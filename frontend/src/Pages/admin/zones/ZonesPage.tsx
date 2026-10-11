import { useState, useEffect } from "react"
import type { Zone } from "../../../models/Zone"
import { getZones, deleteZone } from "../../../services/admin/zoneService"
import ZonesTable from "./ZonesTable"
import ZoneForm from "./ZoneForm"

type View = "LIST" | "CREATE" | "EDIT"

type Props = {
  onBack: () => void
}

export default function ZonesPage(onBack: Props) {
  const [zones, setZones] = useState<Zone[]>([])
  const [selectedZone, setSelectedZone] = useState<Zone | null>(null)
  const [view, setView] = useState<View>("LIST")
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    fetchZones()
  }, [])

  const fetchZones = async () => {
    setIsLoading(true)
    try {
      const res = await getZones()
      console.log("Fetched zones:", res)
      setZones(res)
    } catch (err) {
      console.error(err)
    } finally {
      setIsLoading(false)
    }
  }

  const handleDelete = async (id: string) => {
    if (!window.confirm("Delete this zone?")) return
    try {
      await deleteZone(id)
      setZones(prev => prev.filter(z => z.id !== id))
    } catch (err) {
      console.error(err)
      alert("Failed to delete zone")
    }
  }

  const handleSuccess = () => {
    setView("LIST")
    fetchZones()
  }

  if (view === "CREATE") return <ZoneForm mode="create" onSuccess={handleSuccess} />
  if (view === "EDIT" && selectedZone) return <ZoneForm mode="edit" initialData={selectedZone} onSuccess={handleSuccess} />

  return (
    <ZonesTable
      zones={zones}
      isLoading={isLoading}
      onCreate={() => setView("CREATE")}
      onEdit={z => { setSelectedZone(z); setView("EDIT") }}
      onDelete={handleDelete}
    />
  )
}