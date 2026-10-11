import axios from "axios"
import { getToken } from "../../utils/auth"

const API_URL_RaceDays = "https://localhost:7242/api/RaceDays"

export const createRaceDay = async (raceEventId: string, data: any) => {
  const res = await axios.post(`${API_URL_RaceDays}`, { ...data, raceEventId }, {
    headers: { Authorization: `Bearer ${getToken()}` }
  })
  return res.data
}

export const updateRaceDay = async (id: string, data: any) => {
  const res = await axios.put(`${API_URL_RaceDays}/${id}`, data, {
    headers: { Authorization: `Bearer ${getToken()}` }
  })
  return res.data
}

export const deleteRaceDay = async (id: string) => {
  const res = await axios.delete(`${API_URL_RaceDays}/${id}`, {
    headers: { Authorization: `Bearer ${getToken()}` }
  })
  return res.data
}

export const getRaceDays = async (raceEventId: string) => {
  const res = await axios.get(`${API_URL_RaceDays}/by-event/${raceEventId}`, {
    headers: { Authorization: `Bearer ${getToken()}` }
  })
  return res.data
}