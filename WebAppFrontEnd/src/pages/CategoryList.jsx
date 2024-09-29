import React, { useEffect, useState } from "react";
import { Table } from "antd";
import { useDispatch, useSelector } from "react-redux";
import { Link } from "react-router-dom";
import { BiEdit } from "react-icons/bi";
import { TiDeleteOutline } from "react-icons/ti";
import {
  getCategories,
  deleteProductCategory,
  resetState,
} from "../features/prodCategory/prodCategorySlice";
import CustomModal from "../components/CustomModal";

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
    title: "Action",
    dataIndex: "action",
  },
];

const CategoryList = () => {
  const [open, setOpen] = useState(false);
  const [productCategoryId, setProductCategoryId] = useState("");

  const showModal = (e) => {
    setOpen(true);
    setProductCategoryId(e);
  };
  const hideModal = () => {
    setOpen(false);
  };

  const dispatch = useDispatch();
  useEffect(() => {
    dispatch(resetState());
    dispatch(getCategories());
    // eslint-disable-next-line
  }, []);

  const prodCategoryState = useSelector(
    (state) => state.prodCategory.prodCategories
  );

  const data = [];
  for (let i = 0; i < prodCategoryState.length; i++) {
    data.push({
      key: i + 1,
      title: prodCategoryState[i].title,
      action: (
        <>
          <Link
            to={`/admin/category/${prodCategoryState[i]._id}`}
            className="text-success fs-3"
          >
            <BiEdit />
          </Link>

          <button
            className="ms-3 text-danger fs-3 bg-transparent border-0"
            onClick={() => showModal(prodCategoryState[i]._id)}
          >
            <TiDeleteOutline />
          </button>
        </>
      ),
    });
  }

  const deleteProdCategory = (e) => {
    dispatch(deleteProductCategory(e));
    setOpen(false);
    setTimeout(() => {
      dispatch(getCategories());
    }, 100);
  };
  return (
    <div>
      <h3 className="mb-4 title">Product Categories</h3>
      <div>
        <Table columns={columns} dataSource={data} />
      </div>
      <CustomModal
        hideModal={hideModal}
        open={open}
        performAction={() => {
          deleteProdCategory(productCategoryId);
        }}
        title="Are you sure you want to delete this category?"
      />
    </div>
  );
};

export default CategoryList;
