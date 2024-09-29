import { axiosInstance, config } from "../../utils/axiosConfig";
import { base_url } from "../../utils/base_url";

const getBrands = async () => {
  const response = await axiosInstance.get(`${base_url}brand/`);

  return response.data;
};

const createBrand = async (brand) => {
  const response = await axiosInstance.post(
    `${base_url}brand/`,
    brand,
    config()
  );

  return response.data;
};

const updateBrand = async (brand) => {
  const response = await axiosInstance.put(
    `${base_url}brand/${brand.id}`,
    { title: brand.brandData.title },
    config()
  );

  return response.data;
};

const getBrand = async (id) => {
  const response = await axiosInstance.get(`${base_url}brand/${id}`, config());

  return response.data;
};

const deleteABrand = async (id) => {
  const response = await axiosInstance.delete(
    `${base_url}brand/${id}`,
    config()
  );

  return response.data;
};

const brandService = {
  getBrands,
  createBrand,
  getBrand,
  updateBrand,
  deleteABrand,
};

export default brandService;
