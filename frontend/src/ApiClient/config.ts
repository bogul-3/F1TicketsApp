import { getToken } from "../utils/auth";

const API_BASE = "https://localhost:7242/api/";

export const createRequestWithAuthHeaders = async (
  url = "",
  type: string = "GET",
  payload:  object| undefined = {},
) => {
  const token = getToken();

  if (!token) {
    throw Error("No token");
  }

  let config = {
    method: type,
    headers: { "Authorization ": token },
  };

  if (payload) {
    config["body"] = JSON.stringify(payload);
  }

  return await fetch(API_BASE + url);
};
