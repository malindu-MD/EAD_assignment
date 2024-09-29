import axios from "axios";
import { axiosInstance, config } from "../../utils/axiosConfig";
import { base_url } from "../../utils/base_url";
const login = async (user) => {
  const response = await axios.post(
    `${base_url}user/admin-login`,
    user,
    config()
  );
  if (response.data) {
    localStorage.setItem("user", JSON.stringify(response.data));
  }
  return response.data;
};
const getOrders = async () => {
  const response = await axiosInstance.get(
    `${base_url}user/get-all-orders`,
    config()
  );

  return response.data;
};

const getOrder = async (id) => {
  const response = await axiosInstance.get(
    `${base_url}user/get-order/${id}`,

    config()
  );

  return response.data;
};
const updateOrder = async (data) => {
  const response = await axiosInstance.put(
    `${base_url}user/update-order/${data.id}`,
    { status: data.status },
    config()
  );

  return response.data;
};

const getMonthlyOrders = async () => {
  const response = await axiosInstance.get(
    `${base_url}user/getMonthWiseOrderIncome`,

    config()
  );

  return response.data;
};

const getYearlyStats = async () => {
  const response = await axiosInstance.get(
    `${base_url}user/getyearlyorders`,

    config()
  );

  return response.data;
};

const authService = {
  login,
  getOrders,
  getOrder,
  getMonthlyOrders,
  getYearlyStats,
  updateOrder,
};

export default authService;
