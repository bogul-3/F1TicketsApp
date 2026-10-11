import { useState } from "react"
import type { Zone } from "../../../models/Zone"
import { createZone, updateZone } from "../../../services/admin/zoneService"

type Props =
  | { mode: "create"; initialData?: never; onSuccess: () => void }
  | { mode: "edit"; initialData: Zone; onSuccess: () => void }

export default function ZoneForm({ mode, initialData, onSuccess }: Props) {
  const [name, setName] = useState(initialData?.name || "")
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState("")

  const handleSubmit = async () => {
    setError("")

    // 1. Validation
    if (!name.trim()) {
      setError("Zone name is required")
      return
    }

    if (name.length > 40) {
      setError("Zone name is too long (max 40 characters)")
      return
    }

    // 2. Submit
    setIsLoading(true)
    try {
      if (mode === "create") {
        await createZone(name.trim())
        alert("Zone created")
      } else if (mode === "edit" && initialData) {
        await updateZone(initialData.id, name.trim())
        alert("Zone updated")
      }
      onSuccess()
    } catch (err) {
      console.error(err)
      setError("Operation failed")
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <div className="bg-white shadow-lg rounded-lg p-6 max-w-md mx-auto mt-6">
      <h2 className="text-2xl font-bold text-gray-800 mb-6 text-center">
        {mode === "create" ? "Create Zone" : "Edit Zone"}
      </h2>

      <div className="flex flex-col gap-4">
        <input
          type="text"
          className="border border-gray-300 rounded-lg p-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
          placeholder="Zone Name"
          value={name}
          onChange={e => setName(e.target.value)}
        />
        {error && <p className="text-red-600 font-medium mt-1">{error}</p>}
      </div>

      <div className="flex gap-4 mt-6">
        <button
          onClick={handleSubmit}
          disabled={isLoading}
          className={`flex-1 text-white py-2 rounded-lg font-semibold transition-colors duration-200
            ${mode === "create" ? "bg-green-600 hover:bg-green-700" : "bg-blue-600 hover:bg-blue-700"}
            ${isLoading ? "opacity-50 cursor-not-allowed" : ""}`}
        >
          {mode === "create" ? "Create Zone" : "Update Zone"}
        </button>

        <button
          onClick={onSuccess}
          className="flex-1 bg-gray-300 hover:bg-gray-400 text-gray-800 font-semibold py-2 rounded-lg transition-colors duration-200"
        >
          Cancel
        </button>
      </div>
    </div>
  )
}