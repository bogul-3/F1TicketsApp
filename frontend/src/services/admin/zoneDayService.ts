import axios from "axios";
import type { ZoneDay } from "../../models/ZoneDay";

const API_URL_ZoneDays = "https://localhost:7242/api/ZoneDays";

export const getZoneDays = async (raceDayId: string) => {
  const res = await axios.get<ZoneDay[]>(`${API_URL_ZoneDays}/raceday/${raceDayId}`);
  return res.data;
};

export const createZoneDay = async (raceDayId: string, payload: { zoneId: string; capacity: number; priceAddon: number }) => {
  const res = await axios.post<ZoneDay>(`${API_URL_ZoneDays}`, { ...payload, raceDayId });
  return res.data;
};

export const updateZoneDay = async (id: string, payload: { capacity: number; priceAddon: number }) => {
  const res = await axios.put<ZoneDay>(`${API_URL_ZoneDays}/${id}`, payload);
  return res.data;
};

export const deleteZoneDay = async (id: string) => {
  const res = await axios.delete(`${API_URL_ZoneDays}/${id}`);
  return res.data;
};