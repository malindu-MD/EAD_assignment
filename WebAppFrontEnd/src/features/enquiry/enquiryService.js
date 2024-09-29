import { axiosInstance, config } from "../../utils/axiosConfig";
import { base_url } from "../../utils/base_url";

const getEnquiries = async () => {
  const response = await axiosInstance.get(`${base_url}enquiry/`);

  return response.data;
};

const deleteEnquiry = async (id) => {
  const response = await axiosInstance.delete(
    `${base_url}enquiry/${id}`,
    config()
  );

  return response.data;
};

const getEnquiry = async (id) => {
  const response = await axiosInstance.get(`${base_url}enquiry/${id}`);

  return response.data;
};

const updateEnquiry = async (enq) => {
  const response = await axiosInstance.put(
    `${base_url}enquiry/${enq.id}`,
    { status: enq.enqData },
    config()
  );

  return response.data;
};

const enquiryService = {
  getEnquiries,
  deleteEnquiry,
  getEnquiry,
  updateEnquiry,
};

export default enquiryService;
