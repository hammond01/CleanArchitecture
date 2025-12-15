import type {
  ActionType,
  ProColumns,
} from '@ant-design/pro-components';
import {
  FooterToolbar,
  PageContainer,
  ProTable,
} from '@ant-design/pro-components';
import { FormattedMessage, useIntl } from '@umijs/max';
import { Button, message, Popconfirm } from 'antd';
import React, { useRef, useState } from 'react';
import { productApi } from '@/services/api';
import type { Product } from '@/services/api';
import CreateProductForm from './components/CreateProductForm';
import UpdateProductForm from './components/UpdateProductForm';

const ProductList: React.FC = () => {
  const actionRef = useRef<ActionType>();
  const [createModalVisible, setCreateModalVisible] = useState<boolean>(false);
  const [updateModalVisible, setUpdateModalVisible] = useState<boolean>(false);
  const [currentRow, setCurrentRow] = useState<Product>();
  const [selectedRowsState, setSelectedRows] = useState<Product[]>([]);

  const intl = useIntl();
  const [messageApi, contextHolder] = message.useMessage();

  const columns: ProColumns<Product>[] = [
    {
      title: 'ID',
      dataIndex: 'id',
      tip: 'Product ID',
      render: (dom, entity) => {
        return (
          <a
            onClick={() => {
              // Navigate to product detail page
              window.location.href = `/products/${entity.id}`;
            }}
          >
            {dom}
          </a>
        );
      },
    },
    {
      title: 'Product Name',
      dataIndex: 'productName',
      valueType: 'text',
    },
    {
      title: 'Unit Price',
      dataIndex: 'unitPrice',
      valueType: 'money',
      renderText: (val: number) => `$${val?.toFixed(2) || '0.00'}`,
    },
    {
      title: 'Units In Stock',
      dataIndex: 'unitsInStock',
      valueType: 'digit',
    },
    {
      title: 'Units On Order',
      dataIndex: 'unitsOnOrder',
      valueType: 'digit',
    },
    {
      title: 'Reorder Level',
      dataIndex: 'reorderLevel',
      valueType: 'digit',
    },
    {
      title: 'Discontinued',
      dataIndex: 'discontinued',
      valueType: 'switch',
      render: (_dom, entity) => (entity.discontinued ? 'Yes' : 'No'),
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
          title="Are you sure to delete this product?"
          onConfirm={async () => {
            try {
              await productApi.delete(record.id);
              messageApi.success('Product deleted successfully');
              actionRef.current?.reload();
            } catch (_error) {
              messageApi.error('Failed to delete product');
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
      {contextHolder}
      <ProTable<Product>
        headerTitle={intl.formatMessage({
          id: 'pages.searchTable.title',
          defaultMessage: 'Product Management',
        })}
        actionRef={actionRef}
        rowKey="id"
        search={{
          labelWidth: 120,
        }}
        toolBarRender={() => [
          <Button
            type="primary"
            key="primary"
            onClick={() => {
              setCreateModalVisible(true);
            }}
          >
            <FormattedMessage id="pages.searchTable.new" defaultMessage="New" />
          </Button>,
        ]}
        request={async (_params) => {
          try {
            const response = await productApi.getAll();
            if (response.statusCode === 200) {
              return {
                data: response.data || [],
                success: true,
                total: response.data?.length || 0,
              };
            }
            return {
              data: [],
              success: false,
              total: 0,
            };
          } catch (error) {
            console.error('Failed to fetch products:', error);
            return {
              data: [],
              success: false,
              total: 0,
            };
          }
        }}
        columns={columns}
        rowSelection={{
          onChange: (_, selectedRows) => {
            setSelectedRows(selectedRows);
          },
        }}
      />

      <CreateProductForm
        visible={createModalVisible}
        onCancel={() => setCreateModalVisible(false)}
        onSuccess={() => {
          setCreateModalVisible(false);
          actionRef.current?.reload();
        }}
      />

      <UpdateProductForm
        visible={updateModalVisible}
        productId={currentRow?.id}
        onCancel={() => setUpdateModalVisible(false)}
        onSuccess={() => {
          setUpdateModalVisible(false);
          setCurrentRow(undefined);
          actionRef.current?.reload();
        }}
      />

      {selectedRowsState?.length > 0 && (
        <FooterToolbar
          extra={
            <div>
              <FormattedMessage id="pages.searchTable.chosen" defaultMessage="Chosen" />{' '}
              <a style={{ fontWeight: 600 }}>{selectedRowsState.length}</a>{' '}
              <FormattedMessage id="pages.searchTable.item" defaultMessage="项" />
              &nbsp;&nbsp;
            </div>
          }
        >
          <Button
            onClick={async () => {
              try {
                // Delete selected products
                await Promise.all(
                  selectedRowsState.map((product) => productApi.delete(product.id))
                );
                messageApi.success('Products deleted successfully');
                setSelectedRows([]);
                actionRef.current?.reload();
              } catch (_error) {
                messageApi.error('Failed to delete some products');
              }
            }}
          >
            <FormattedMessage
              id="pages.searchTable.batchDeletion"
              defaultMessage="Batch deletion"
            />
          </Button>
        </FooterToolbar>
      )}
    </PageContainer>
  );
};

export default ProductList;
