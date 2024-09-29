import { axiosInstance, config } from "../../utils/axiosConfig";
import { base_url } from "../../utils/base_url";

const getColors = async () => {
  const response = await axiosInstance.get(`${base_url}color/`);

  return response.data;
};

const createColor = async (color) => {
  const response = await axiosInstance.post(
    `${base_url}color/`,
    color,
    config()
  );

  return response.data;
};

const updateColor = async (color) => {
  const response = await axiosInstance.put(
    `${base_url}color/${color.id}`,
    { title: color.colorData.title },
    config()
  );

  return response.data;
};

const getColor = async (id) => {
  const response = await axiosInstance.get(`${base_url}color/${id}`, config());

  return response.data;
};

const deleteColor = async (id) => {
  const response = await axiosInstance.delete(
    `${base_url}color/${id}`,
    config()
  );

  return response.data;
};

const colorService = {
  getColors,
  createColor,
  updateColor,
  getColor,
  deleteColor,
};

export default colorService;
