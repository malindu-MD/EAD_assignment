import { axiosInstance, config } from "../../utils/axiosConfig";
import { base_url } from "../../utils/base_url";

const getCoupons = async () => {
  const response = await axiosInstance.get(`${base_url}coupon/`, config());

  return response.data;
};

const createCoupon = async (coupon) => {
  const response = await axiosInstance.post(
    `${base_url}coupon/`,
    coupon,
    config()
  );

  return response.data;
};

const updateCoupon = async (coupon) => {
  const response = await axiosInstance.put(
    `${base_url}coupon/${coupon.id}`,
    {
      name: coupon.couponData.name,
      expiry: coupon.couponData.expiry,
      discount: coupon.couponData.discount,
    },
    config()
  );

  return response.data;
};

const getCoupon = async (id) => {
  const response = await axiosInstance.get(`${base_url}coupon/${id}`, config());

  return response.data;
};

const deleteACoupon = async (id) => {
  const response = await axiosInstance.delete(
    `${base_url}coupon/${id}`,
    config()
  );

  return response.data;
};

const couponService = {
  getCoupons,
  createCoupon,
  updateCoupon,
  getCoupon,
  deleteACoupon,
};

export default couponService;
