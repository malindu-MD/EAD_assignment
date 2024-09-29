import { axiosInstance, config } from "../../utils/axiosConfig";
import { base_url } from "../../utils/base_url";

const uploadImg = async (data) => {
  const response = await axiosInstance.post(
    `${base_url}upload/`,
    data,
    config()
  );
  return response.data;
};
const deleteImg = async (id) => {
  const response = await axiosInstance.delete(
    `${base_url}upload/delete-img/${id}`,

    config()
  );
  return response.data;
};

const uploadService = {
  uploadImg,
  deleteImg,
};

export default uploadService;
