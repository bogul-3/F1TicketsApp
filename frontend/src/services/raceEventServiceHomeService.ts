import type { RaceEventHome } from "../models/RaceEvent";

const API_URL_RaceEvents = "https://localhost:7242/api/RaceEvents";

export const getRaceEvents = async (): Promise<RaceEventHome[]> => {
  const response = await fetch(`${API_URL_RaceEvents}/full`);

  if (!response.ok) {
    throw new Error("Failed to fetch race events");
  }

  return response.json();
};

export const getRaceEvent = async (id: string) => {
  const response = await fetch(`${API_URL_RaceEvents}/${id}/full`);
  if (!response.ok) throw new Error("Cannot fetch event");
  return response.json();
};