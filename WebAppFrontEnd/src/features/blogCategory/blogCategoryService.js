import { axiosInstance, config } from "../../utils/axiosConfig";
import { base_url } from "../../utils/base_url";

const getBlogCategories = async () => {
  const response = await axiosInstance.get(`${base_url}blog-category/`);

  return response.data;
};

const createBlogCategory = async (blogCategory) => {
  const response = await axiosInstance.post(
    `${base_url}blog-category/`,
    blogCategory,
    config()
  );

  return response.data;
};

const updateBlogCategory = async (blogCategory) => {
  const response = await axiosInstance.put(
    `${base_url}blog-category/${blogCategory.id}`,
    { title: blogCategory.blogCatData.title },
    config()
  );

  return response.data;
};

const getBlogCategory = async (id) => {
  const response = await axiosInstance.get(
    `${base_url}blog-category/${id}`,
    config()
  );

  return response.data;
};

const deleteBlogCategory = async (id) => {
  const response = await axiosInstance.delete(
    `${base_url}blog-category/${id}`,
    config()
  );

  return response.data;
};

const blogCategoryService = {
  getBlogCategories,
  createBlogCategory,
  updateBlogCategory,
  getBlogCategory,
  deleteBlogCategory,
};

export default blogCategoryService;
