export const saveToken = (token: string) => {
  localStorage.setItem("jwt", token);
};

export const getToken = () => {
  return localStorage.getItem("jwt");
};

export const logout = () => {
  localStorage.removeItem("jwt");
};

export const isAuthenticated = () => {
  return !!localStorage.getItem("jwt");
};