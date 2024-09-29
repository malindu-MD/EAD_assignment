import React, { useEffect } from "react";
import { Table } from "antd";
import { useDispatch, useSelector } from "react-redux";
import {
  deleteProduct,
  getProducts,
  resetState,
} from "../features/product/productSlice";
import { Link } from "react-router-dom";
import { BiEdit } from "react-icons/bi";
import { TiDeleteOutline } from "react-icons/ti";
import CustomModal from "../components/CustomModal";
import { useState } from "react";
import { getColors } from "../features/color/colorSlice";

const ProductList = () => {
  const columns = [
    {
      title: "Number",
      dataIndex: "key",
    },
    {
      title: "Title",
      dataIndex: "title",
      sorter: (a, b) => a.title.length - b.title.length,
    },
    {
      title: "Brand",
      dataIndex: "brand",
      sorter: (a, b) => a.brand.length - b.brand.length,
    },
    {
      title: "Category",
      dataIndex: "category",
      sorter: (a, b) => a.category.length - b.category.length,
    },
    {
      title: "Color",
      dataIndex: "color",
      render: (colors) => (
        <div>
          {colors.map((colorId) => (
            <div
              key={colorId}
              style={{
                backgroundColor: colorMap[colorId],
                width: "20px",
                height: "20px",
                borderRadius: "50%",
                display: "inline-block",
                margin: "2px",
              }}
            ></div>
          ))}
        </div>
      ),
    },
    {
      title: "Quantity",
      dataIndex: "quantity",
    },
    {
      title: "Price",
      dataIndex: "price",
      sorter: (a, b) => a.price - b.price,
    },
    {
      title: "Action",
      dataIndex: "action",
    },
  ];

  const [open, setOpen] = useState(false);
  const [productId, setProductId] = useState("");
  const showModal = (e) => {
    setOpen(true);
    setProductId(e);
  };
  const hideModal = () => {
    setOpen(false);
  };

  const dispatch = useDispatch();
  useEffect(() => {
    dispatch(getProducts());
    dispatch(getColors());
    dispatch(resetState());
    // eslint-disable-next-line
  }, []);
  const productState = useSelector((state) => state.product.products);
  const colorState = useSelector((state) => state.color.colors);
  const colorMap = {};
  colorState?.forEach((color) => {
    colorMap[color?._id] = color?.title;
  });

  const data = [];
  for (let i = 0; i < productState.length; i++) {
    data.push({
      key: i + 1,
      title: productState[i].title,
      brand: productState[i].brand,
      category: productState[i].category,
      color: productState[i].color ? productState[i].color : [],
      quantity: productState[i].quantity,
      price: productState[i].price,
      action: (
        <>
          <Link
            to={`/admin/product/${productState[i]._id}`}
            className="text-success fs-3"
          >
            <BiEdit />
          </Link>

          <button
            className="ms-3 text-danger fs-3 bg-transparent border-0"
            onClick={() => showModal(productState[i]._id)}
          >
            <TiDeleteOutline />
          </button>
        </>
      ),
    });
  }

  const deleteAProduct = (e) => {
    dispatch(deleteProduct(e));
    setOpen(false);
    setTimeout(() => {
      dispatch(getProducts());
    }, 100);
  };

  return (
    <div>
      <h3 className="mb-4 title">Products</h3>
      <div>
        <Table columns={columns} dataSource={data} />
      </div>
      <CustomModal
        hideModal={hideModal}
        open={open}
        performAction={() => {
          deleteAProduct(productId);
        }}
        title="Are you sure you want to delete the Product?"
      />
    </div>
  );
};

export default ProductList;
