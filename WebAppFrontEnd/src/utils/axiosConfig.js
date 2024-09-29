import axios from "axios";

const getTokenFromLocalStorage = () => {
  const userData = localStorage.getItem("user");
  return userData ? JSON.parse(userData) : null;
};

export const config = () => ({
  headers: {
    Authorization: `Bearer ${
      getTokenFromLocalStorage() !== null
        ? getTokenFromLocalStorage().token
        : ""
    }`,
    Accept: "application/json",
  },
});

export const axiosInstance = axios.create({
  baseURL: "http://localhost:8000/api/",
});

// Request interceptor for API calls
axiosInstance.interceptors.request.use(
  async (config) => {
    const customerData = localStorage.getItem("user");
    const token = customerData ? JSON.parse(customerData).token : null;
    if (token) {
      config.headers = {
        Authorization: `Bearer ${token}`,
        Accept: "application/json",
      };
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);
