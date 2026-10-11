import { useState, useEffect } from "react"
import type { Country, City } from "../../../models/Location"
import type { RaceEvent } from "../../../models/RaceEvent"

import { getAllCountries, getCitiesByCountry } from "../../../services/locationService"
import { createRaceEvent, updateRaceEvent } from "../../../services/admin/raceEventServiceAdmin"

type RaceEventFormProps =
  | { mode: "create"; initialData?: never; onSuccess: () => void }
  | { mode: "edit"; initialData: RaceEvent; onSuccess: () => void }

export default function RaceEventForm({ mode, initialData, onSuccess }: RaceEventFormProps) {

  // --- Helper za konverziju ISO string -> input format ---
  const toInputDateTime = (iso?: string) => {
    if (!iso) return ""
    const d = new Date(iso)
    const pad = (n: number) => n.toString().padStart(2, "0")
    return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`
  }

  const toInputDate = (iso?: string) => iso?.split("T")[0] || ""
  const formatToISO = (value: string) => value ? new Date(value).toISOString() : ""

  // --- Form state ---
  const [name, setName] = useState(initialData?.name || "")
  const [startDate, setStartDate] = useState(initialData ? toInputDateTime(initialData.startDate) : "")
  const [endDate, setEndDate] = useState(initialData ? toInputDateTime(initialData.endDate) : "")
  const [additionalInfo, setAdditionalInfo] = useState(initialData?.additionalInfo || "")
  const [discountValidUntil, setDiscountValidUntil] = useState(initialData ? toInputDate(initialData.discountValidUntil) : "")

  const [countries, setCountries] = useState<Country[]>([])
  const [cities, setCities] = useState<City[]>([])

  const [selectedCountry, setSelectedCountry] = useState("")
  const [selectedCity, setSelectedCity] = useState(initialData?.cityId || "")

  const [isSubmitting, setIsSubmitting] = useState(false)
  const [error, setError] = useState("")

  // --- Fetch countries ---
  useEffect(() => {
    getAllCountries().then(setCountries).catch(console.error)
  }, [])

  // --- Fetch cities kada se menja zemlja ---
  useEffect(() => {
    if (!selectedCountry) {
      setCities([])
      setSelectedCity("")
      return
    }
    getCitiesByCountry(selectedCountry).then(setCities).catch(console.error)
  }, [selectedCountry])

  // --- Submit handler ---
  const handleSubmit = async () => {
    setError("")

    // 1. Required fields
    if (!name.trim()) {
      setError("Event name is required")
      return
    }

    if (!startDate) {
      setError("Start date is required")
      return
    }

    if (!endDate) {
      setError("End date is required")
      return
    }

    if (!selectedCountry) {
      setError("Country is required")
      return
    }

    if (!selectedCity) {
      setError("City is required")
      return
    }

    // 2. Max lengths
    if (name.length > 40) {
      setError("Event name is too long (max 40 characters)")
      return
    }

    if (additionalInfo.length > 500) {
      setError("Additional info is too long (max 500 characters)")
      return
    }

    // 3. Date validations
    const start = new Date(formatToISO(startDate))
    const end = new Date(formatToISO(endDate))
    const discount = discountValidUntil
      ? new Date(discountValidUntil + "T00:00:00Z")
      : null;

    if (start > end) {
      setError("Start date cannot be after End date")
      return
    }

    if (discount && discount > start) {
      setError("Discount valid until cannot be after Start date")
      return
    }

    // 4. Submit to backend
    setIsSubmitting(true)
    try {
      const payload = {
        name: name.trim(),
        startDate: formatToISO(startDate),
        endDate: formatToISO(endDate),
        additionalInfo: additionalInfo.trim(),
        discountValidUntil: discount ? discount.toISOString() : null,
        cityId: selectedCity
      }

      if (mode === "create") {
        await createRaceEvent(payload)
        alert("Race event created")
      } else {
        if (!initialData) return
        await updateRaceEvent(initialData.id, payload)
        alert("Race event updated")
      }

      onSuccess()
    } catch (err: any) {
      console.error("Failed to submit race event:", err)
      setError(err.response?.data?.message || "Failed to submit race event")
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <div className="bg-white shadow-lg rounded-lg p-6 max-w-md mx-auto mt-6">
      <h2 className="text-2xl font-bold text-gray-800 mb-6 text-center">
        {mode === "create" ? "Create Race Event" : "Edit Race Event"}
      </h2>

      <div className="flex flex-col gap-4">
        <input
          className="border border-gray-300 rounded-lg p-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
          placeholder="Event name"
          value={name}
          onChange={e => setName(e.target.value)}
        />

        <input
          type="datetime-local"
          className="border border-gray-300 rounded-lg p-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
          value={startDate}
          onChange={e => setStartDate(e.target.value)}
        />

        <input
          type="datetime-local"
          className="border border-gray-300 rounded-lg p-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
          value={endDate}
          onChange={e => setEndDate(e.target.value)}
        />

        <input
          className="border border-gray-300 rounded-lg p-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
          placeholder="Additional info"
          value={additionalInfo}
          onChange={e => setAdditionalInfo(e.target.value)}
        />

        <input
          type="date"
          className="border border-gray-300 rounded-lg p-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
          value={discountValidUntil}
          onChange={e => setDiscountValidUntil(e.target.value)}
        />

        {/* COUNTRY */}
        <select
          className="border border-gray-300 rounded-lg p-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
          value={selectedCountry}
          onChange={e => setSelectedCountry(e.target.value)}
        >
          <option value="">Select country</option>
          {countries.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
        </select>

        {/* CITY */}
        <select
          className="border border-gray-300 rounded-lg p-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
          value={selectedCity}
          onChange={e => setSelectedCity(e.target.value)}
          disabled={!selectedCountry}
        >
          <option value="">Select city</option>
          {cities.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
        </select>

        {/* ERROR */}
        {error && <p className="text-red-600 font-medium mt-1">{error}</p>}
      </div>

      <div className="flex gap-4 mt-6">
        <button
          onClick={handleSubmit}
          disabled={isSubmitting}
          className={`flex-1 text-white py-2 rounded-lg font-semibold transition-colors duration-200
            ${mode === "create" ? "bg-green-600 hover:bg-green-700" : "bg-blue-600 hover:bg-blue-700"}
            ${isSubmitting ? "opacity-50 cursor-not-allowed" : ""}`}
        >
          {isSubmitting ? (mode === "create" ? "Creating..." : "Updating...") : (mode === "create" ? "Create Event" : "Update Event")}
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