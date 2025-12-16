import type { ActionType, ProColumns } from '@ant-design/pro-components';
import { PageContainer, ProTable } from '@ant-design/pro-components';
import { FormattedMessage } from '@umijs/max';
import { Button, message, Popconfirm } from 'antd';
import React, { useRef, useState } from 'react';
import { categoryApi } from '@/services/api';
import type { Category } from '@/services/api';
import CreateCategoryForm from './components/CreateCategoryForm';
import UpdateCategoryForm from './components/UpdateCategoryForm';

const CategoryList: React.FC = () => {
  const actionRef = useRef<ActionType>(null);
  const [createModalVisible, setCreateModalVisible] = useState<boolean>(false);
  const [updateModalVisible, setUpdateModalVisible] = useState<boolean>(false);
  const [currentRow, setCurrentRow] = useState<Category>();
  const [selectedRowsState, setSelectedRows] = useState<Category[]>([]);

  const columns: ProColumns<Category>[] = [
    {
      title: 'ID',
      dataIndex: 'id',
      render: (dom, entity) => (
        <a
          onClick={() => {
            window.location.href = `/categories/${entity.id}`;
          }}
        >
          {dom}
        </a>
      ),
    },
    {
      title: 'Category Name',
      dataIndex: 'categoryName',
      valueType: 'text',
    },
    {
      title: 'Description',
      dataIndex: 'description',
      valueType: 'text',
      responsive: ['lg'],
    },
    {
      title: 'Actions',
      dataIndex: 'option',
      valueType: 'option',
      render: (_, record) => [
        <a
          key="edit"
          onClick={() => {
            setCurrentRow(record);
            setUpdateModalVisible(true);
          }}
        >
          <FormattedMessage id="pages.searchTable.edit" defaultMessage="Edit" />
        </a>,
        <Popconfirm
          key="delete"
          title="Are you sure to delete this category?"
          onConfirm={async () => {
            try {
              await categoryApi.delete(record.id);
              message.success('Category deleted successfully');
              actionRef.current?.reload();
            } catch (_error) {
              message.error('Failed to delete category');
            }
          }}
          okText="Yes"
          cancelText="No"
        >
          <a key="delete" style={{ color: 'red' }}>
            <FormattedMessage id="pages.searchTable.delete" defaultMessage="Delete" />
          </a>
        </Popconfirm>,
      ],
    },
  ];

  return (
    <PageContainer>
      <ProTable<Category>
        headerTitle={<FormattedMessage id="pages.searchTable.title" defaultMessage="Category Management" />}
        actionRef={actionRef}
        rowKey="id"
        request={async () => {
          try {
            const response = await categoryApi.getAll();
            if (response.statusCode === 200) {
              const data = response.data || [];
              return { data, success: true, total: data.length };
            }
            return { data: [], success: false, total: 0 };
          } catch (_error) {
            return { data: [], success: false, total: 0 };
          }
        }}
        columns={columns}
        toolBarRender={() => [
          <Button
            type="primary"
            key="primary"
            onClick={() => setCreateModalVisible(true)}
          >
            <FormattedMessage id="pages.searchTable.new" defaultMessage="New" />
          </Button>,
        ]}
        rowSelection={{
          onChange: (_, selectedRows) => {
            setSelectedRows(selectedRows);
          },
        }}
      />

      <CreateCategoryForm
        visible={createModalVisible}
        onCancel={() => setCreateModalVisible(false)}
        onSuccess={() => {
          setCreateModalVisible(false);
          actionRef.current?.reload();
        }}
      />

      <UpdateCategoryForm
        visible={updateModalVisible}
        categoryId={currentRow?.id}
        onCancel={() => setUpdateModalVisible(false)}
        onSuccess={() => {
          setUpdateModalVisible(false);
          setCurrentRow(undefined);
          actionRef.current?.reload();
        }}
      />

      {selectedRowsState?.length > 0 && (
        <div style={{ marginTop: 16 }}>
          <Button
            onClick={async () => {
              try {
                await Promise.all(selectedRowsState.map((c) => categoryApi.delete(c.id)));
                message.success('Categories deleted successfully');
                setSelectedRows([]);
                actionRef.current?.reload();
              } catch (_error) {
                message.error('Failed to delete some categories');
              }
            }}
          >
            <FormattedMessage id="pages.searchTable.batchDeletion" defaultMessage="Batch deletion" />
          </Button>
        </div>
      )}
    </PageContainer>
  );
};

export default CategoryList;
