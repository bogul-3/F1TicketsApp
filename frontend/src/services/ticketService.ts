import type { TicketModel } from "../models/TicketModel";
import axios from "axios";

export interface PurchaseRequest {
  zoneDayId: string;
  userId: string;
  quantity: number;
  promoCode?: string | null;
  currencyCode?: string;
}

export interface PurchaseResponse {
  purchaseRequestId: string;
  totalPrice: number;
  promoCode: string;
  promoCodeId: string;
  currencyCode: string;
}

export const purchaseTickets = async (data: PurchaseRequest): Promise<PurchaseResponse> => {
  const response = await fetch("https://localhost:7242/api/Ticket/purchase", { 
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });

  if (!response.ok) {
    const errMsg = await response.text();
    throw new Error(errMsg || "Purchase failed");
  }

  const result: PurchaseResponse = await response.json();
  return result;
};

export async function getUserTickets(userId: string): Promise<TicketModel[]> {
  try {
    const response = await fetch(`https://localhost:7242/api/Ticket/by-user/${userId}`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        // Ako koristiš JWT auth
        "Authorization": `Bearer ${localStorage.getItem("token")}`,
      },
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error?.error || "Failed to fetch tickets");
    }

    const data: TicketModel[] = await response.json();
    return data;
  } catch (err: any) {
    console.error("Error fetching tickets by user:", err);
    throw err;
  }
}

export const updateTicket = async (payload: {
  ticketId: string;
  accessCode: string;
  email: string;
  addZoneDayId: string;
  removeZoneDayId: string;
}) => {
  console.log(payload)
  const response = await axios.post(
    "https://localhost:7242/api/Ticket/update",
    payload
  );
  return response.data;
};

// src/services/ticketService.ts
export async function cancelTicket(ticket: {
  ticketId: string;
  accessCode: string;
  email: string;
}) {
  const response = await fetch("https://localhost:7242/api/Ticket/delete", {
    method: "DELETE",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(ticket),
  });
  if (!response.ok) throw new Error("Failed to cancel ticket");
  return await response.json();
}