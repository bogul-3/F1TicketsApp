import axios from "axios"
import { getToken } from "../../utils/auth"

const API_URL_RaceEvents = "https://localhost:7242/api/RaceEvents"

export const getAllRaceEvents = async () => {
  const res = await axios.get(`${API_URL_RaceEvents}`, {
    headers: { Authorization: `Bearer ${getToken()}` }
  })
  return res.data
}

export const createRaceEvent = async (data: any) => {
  const res = await axios.post(`${API_URL_RaceEvents}`, data, {
    headers: { Authorization: `Bearer ${getToken()}` }
  })
  return res.data
}

export const updateRaceEvent = async (id: string, data: any) => {
  const res = await axios.put(`${API_URL_RaceEvents}/${id}`, data, {
    headers: { Authorization: `Bearer ${getToken()}` }
  })
  return res.data
}

export const deleteRaceEvent = async (id: string) => {
  const res = await axios.delete(`${API_URL_RaceEvents}/${id}`, {
    headers: { Authorization: `Bearer ${getToken()}` }
  })
  return res.data
}