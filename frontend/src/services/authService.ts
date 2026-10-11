import type { SignupResponseDto } from "../models/Auth";

export type LoginRequest ={
    email: string;
    password: string;
}

export type RegisterRequest = {
    name: string;
    surname: string;
    email: string;
    password: string;
    address: string;
    postalCode: string;
    cityId: string;
}

export async function loginUser(data: LoginRequest) {
  const res = await fetch("https://localhost:7242/api/Auth/login", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  })

  if (!res.ok) {
    const err = await res.json()
    throw new Error(err.message || "Login failed")
  }

  return res.json() // { token: string }
}

export const registerUser = async (data: any): Promise<SignupResponseDto> => {
  const res = await fetch("https://localhost:7242/api/Auth/signup", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(data),
  })

  if (!res.ok) {
    const text = await res.text()
    throw new Error(text || "Registration failed")
  }

  const json: SignupResponseDto = await res.json()
  return json
}