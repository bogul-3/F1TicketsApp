import axios from "axios"
import type { Zone } from "../../models/Zone"

const API_URL_Zones = "https://localhost:7242/api/Zones"

export const getZones = async (): Promise<Zone[]> => {
  const res = await axios.get(API_URL_Zones)
  return res.data
}

export const createZone = async (name: string): Promise<void> => {
  await axios.post(`${API_URL_Zones}?name=${name}`)
}

export const updateZone = async (id: string, name: string): Promise<void> => {
  await axios.put(`${API_URL_Zones}/${id}?name=${name}`)
}

export const deleteZone = async (id: string): Promise<void> => {
  await axios.delete(`${API_URL_Zones}/${id}`)
}