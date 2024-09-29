import { axiosInstance, config } from "../../utils/axiosConfig";
import { base_url } from "../../utils/base_url";

const getProductCategories = async () => {
  const response = await axiosInstance.get(`${base_url}category/`);

  return response.data;
};

const createProdCategory = async (prodCategory) => {
  const response = await axiosInstance.post(
    `${base_url}category/`,
    prodCategory,
    config()
  );

  return response.data;
};

const getproductCategory = async (id) => {
  const response = await axiosInstance.get(
    `${base_url}category/${id}`,
    config()
  );

  return response.data;
};

const updateProductCategory = async (category) => {
  const response = await axiosInstance.put(
    `${base_url}category/${category.id}`,
    { title: category.prodCatData.title },
    config()
  );

  return response.data;
};

const deleteProductCategory = async (id) => {
  const response = await axiosInstance.delete(
    `${base_url}category/${id}`,
    config()
  );

  return response.data;
};

const prodCategoryService = {
  getProductCategories,
  createProdCategory,
  getproductCategory,
  deleteProductCategory,
  updateProductCategory,
};

export default prodCategoryService;
