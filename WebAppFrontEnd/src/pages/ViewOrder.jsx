import React, { useEffect } from "react";
import { Table } from "antd";
import { useDispatch, useSelector } from "react-redux";
import { useLocation, useNavigate } from "react-router-dom";
import { getAOrder } from "../features/auth/authSlice";
import { HiOutlineArrowLongLeft } from "react-icons/hi2";

const columns = [
  {
    title: "Number",
    dataIndex: "key",
  },
  {
    title: "Name",
    dataIndex: "name",
  },
  {
    title: "Brand",
    dataIndex: "brand",
  },
  {
    title: "Category",
    dataIndex: "category",
  },
  {
    title: "Count",
    dataIndex: "count",
  },
  {
    title: "Amount",
    dataIndex: "amount",
  },
  {
    title: "Color",
    dataIndex: "color",
    render: (color) => (
      <div
        style={{
          backgroundColor: color,
          width: "20px",
          height: "20px",
          borderRadius: "50%",
        }}
      ></div>
    ),
  },
  {
    title: "Date",
    dataIndex: "date",
  },
];

const ViewOrder = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const orderId = location.pathname.split("/")[3];
  const dispatch = useDispatch();
  useEffect(() => {
    dispatch(getAOrder(orderId));
    // eslint-disable-next-line
  }, []);
  const goBack = () => {
    navigate(-1);
  };
  const orderState = useSelector((state) => state?.auth?.singleOrder?.orders);
  const data = [];
  for (let i = 0; i < orderState?.orderItems?.length; i++) {
    data.push({
      key: i + 1,
      name: orderState?.orderItems[i]?.product?.title,
      brand: orderState?.orderItems[i]?.product?.brand,
      category: orderState?.orderItems[i]?.product?.category,
      count: orderState?.orderItems[i]?.quantity,
      amount: orderState?.orderItems[i]?.price,
      color: orderState?.orderItems[i]?.color?.title,
      date: new Date(
        orderState?.orderItems[i]?.product?.createdAt
      ).toLocaleString(),
    });
  }

  return (
    <div>
      <div className="d-flex  align-items-center justify-content-between">
        <h3 className="mb-4 title">View Order</h3>
        <button
          className="bg-transparent border-0 mb-0 fs-6 d-flex gap-3 align-items-center"
          onClick={goBack}
        >
          <HiOutlineArrowLongLeft className="text-dark fs-2" />
          Go Back
        </button>
      </div>

      <div>{<Table columns={columns} dataSource={data} />}</div>
    </div>
  );
};

export default ViewOrder;
