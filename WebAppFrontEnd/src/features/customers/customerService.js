import { base_url } from "../../utils/base_url";
import { axiosInstance } from "../../utils/axiosConfig";

const getUsers = async () => {
  const response = await axiosInstance.get(`${base_url}user/all-users`);

  return response.data;
};

const customerService = {
  getUsers,
};

export default customerService;
