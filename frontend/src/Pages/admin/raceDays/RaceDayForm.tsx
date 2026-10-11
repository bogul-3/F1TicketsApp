import { useState } from "react"
import type { RaceDay } from "../../../models/RaceDay"
import { createRaceDay, updateRaceDay } from "../../../services/admin/raceDayService"

type Props =
  | { mode: "create"; raceEventId: string; onSuccess: () => void; startDate: string; endDate: string }
  | { mode: "edit"; initialData: RaceDay; onSuccess: () => void; startDate: string; endDate: string }

export default function RaceDayForm(props: Props) {
  const { mode, onSuccess, startDate, endDate } = props
  const raceEventId = mode === "create" ? props.raceEventId : props.initialData.raceEventId
  const initialData = mode === "edit" ? props.initialData : undefined

  const [date, setDate] = useState(initialData?.date.slice(0, 10) || "")
  const [basePrice, setBasePrice] = useState(initialData?.basePrice || 0)
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState("")

  const handleSubmit = async () => {
    setError("")

    // 1. Required fields
    if (!date) {
      setError("Race date is required")
      return
    }

    if (basePrice === undefined || basePrice === null) {
      setError("Base price is required")
      return
    }

    // 2. BasePrice validation
    if (basePrice <= 0) {
      setError("Base price cannot be negative or zero")
      return
    }

    // 3. Date within RaceEvent validation
    const selectedDate = new Date(date + "T18:00:00Z"); // treat as UTC
    const start = new Date(startDate);
    const end = new Date(endDate);
    
    if (selectedDate < start || selectedDate > end) {
      setError(`Race date must be between ${start.toLocaleDateString()} and ${end.toLocaleDateString()}`);
      return;
    }

    // 4. Submit to backend
    setIsLoading(true)
    try {
      const payload = { date: selectedDate.toISOString(), basePrice }
      if (mode === "create") {
        await createRaceDay(raceEventId, payload)
        alert("Race day created")
      }
      if (mode === "edit" && initialData) {
        await updateRaceDay(initialData.id, payload)
        alert("Race day updated")
      }
      onSuccess()
    } catch (err) {
      console.error(err)
      alert(`Operation failed: ${err.toString()}`)
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <div className="bg-white shadow-lg rounded-lg p-6 max-w-md mx-auto mt-6">
      <h2 className="text-2xl font-bold text-gray-800 mb-6 text-center">
        {mode === "create" ? "Create Race Day" : "Edit Race Day"}
      </h2>

      <div className="flex flex-col gap-4">
        {/* Date */}
        <div className="flex flex-col">
          <label htmlFor="raceDate" className="text-gray-700 font-medium mb-1">
            Race Date
          </label>
          <input
            id="raceDate"
            type="date"
            className="border border-gray-300 rounded-lg p-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
            value={date}
            onChange={e => setDate(e.target.value)}
          />
        </div>

        {/* Base Price */}
        <div className="flex flex-col">
          <label htmlFor="basePrice" className="text-gray-700 font-medium mb-1">
            Base Price
          </label>
          <input
            id="basePrice"
            type="number"
            min={0}
            className="border border-gray-300 rounded-lg p-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
            value={basePrice}
            onChange={e => setBasePrice(Number(e.target.value))}
            placeholder="Base Price"
          />
        </div>

        {/* Error message */}
        {error && <p className="text-red-600 font-medium mt-1">{error}</p>}
      </div>

      {/* Buttons */}
      <div className="flex gap-4 mt-6">
        <button
          onClick={handleSubmit}
          disabled={isLoading}
          className={`flex-1 text-white py-2 rounded-lg font-semibold transition-colors duration-200
            ${mode === "create" ? "bg-green-600 hover:bg-green-700" : "bg-blue-600 hover:bg-blue-700"}
            ${isLoading ? "opacity-50 cursor-not-allowed" : ""}`}
        >
          {mode === "create" ? "Create Day" : "Update Day"}
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