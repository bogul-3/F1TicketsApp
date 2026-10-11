import type { Country, City } from "../models/Location"
const API_BASE = "https://localhost:7242/api/location"

export const getAllCountries = async (): Promise<Country[]> => {
  const res = await fetch(`${API_BASE}/countries`)
  if (!res.ok) throw new Error("Failed to fetch countries")
  return res.json()
}

export const getCitiesByCountry = async (countryId: string): Promise<City[]> => {
  const res = await fetch(`${API_BASE}/cities/${countryId}`)
  if (!res.ok) throw new Error("Failed to fetch cities")
  return res.json()
}