import type { ActionType, ProColumns } from '@ant-design/pro-components';
import { PageContainer, ProTable } from '@ant-design/pro-components';
import { FormattedMessage } from '@umijs/max';
import { Button, message, Popconfirm } from 'antd';
import React, { useRef, useState } from 'react';
import { supplierApi } from '@/services/api';
import type { Supplier } from '@/services/api';
import CreateSupplierForm from './components/CreateSupplierForm';
import UpdateSupplierForm from './components/UpdateSupplierForm';

const SupplierList: React.FC = () => {
  const actionRef = useRef<ActionType>(null);
  const [createModalVisible, setCreateModalVisible] = useState<boolean>(false);
  const [updateModalVisible, setUpdateModalVisible] = useState<boolean>(false);
  const [currentRow, setCurrentRow] = useState<Supplier>();
  const [selectedRowsState, setSelectedRows] = useState<Supplier[]>([]);

  const columns: ProColumns<Supplier>[] = [
    { title: 'ID', dataIndex: 'id' },
    { title: 'Company', dataIndex: 'companyName' },
    { title: 'Contact', dataIndex: 'contactName', responsive: ['md'] },
    { title: 'Phone', dataIndex: 'phone', responsive: ['sm'] },
    { title: 'Country', dataIndex: 'country', responsive: ['lg'] },
    {
      title: 'Actions',
      dataIndex: 'option',
      valueType: 'option',
      render: (_, record) => [
        <a key="edit" onClick={() => { setCurrentRow(record); setUpdateModalVisible(true); }}>
          <FormattedMessage id="pages.searchTable.edit" defaultMessage="Edit" />
        </a>,
        <Popconfirm key="delete" title="Are you sure to delete this supplier?" onConfirm={async () => {
          try { await supplierApi.delete(record.id); message.success('Supplier deleted'); actionRef.current?.reload(); }
          catch { message.error('Failed to delete supplier'); }
        }} okText="Yes" cancelText="No">
          <a key="delete" style={{ color: 'red' }}>Delete</a>
        </Popconfirm>,
      ],
    },
  ];

  return (
    <PageContainer>
      <ProTable<Supplier>
        headerTitle={<FormattedMessage id="pages.supplier.title" defaultMessage="Supplier Management" />}
        actionRef={actionRef}
        rowKey="id"
        request={async () => {
          try {
            const res = await supplierApi.getAll();
            if (res.statusCode === 200) return { data: res.data || [], success: true, total: (res.data || []).length };
            return { data: [], success: false, total: 0 };
          } catch { return { data: [], success: false, total: 0 }; }
        }}
        columns={columns}
        toolBarRender={() => [<Button key="new" type="primary" onClick={() => setCreateModalVisible(true)}>New</Button>]}
        rowSelection={{ onChange: (_, rows) => setSelectedRows(rows) }}
      />

      <CreateSupplierForm visible={createModalVisible} onCancel={() => setCreateModalVisible(false)} onSuccess={() => { setCreateModalVisible(false); actionRef.current?.reload(); }} />

      <UpdateSupplierForm visible={updateModalVisible} supplierId={currentRow?.id} onCancel={() => setUpdateModalVisible(false)} onSuccess={() => { setUpdateModalVisible(false); setCurrentRow(undefined); actionRef.current?.reload(); }} />

      {selectedRowsState?.length > 0 && (
        <div style={{ marginTop: 16 }}>
          <Button onClick={async () => { try { await Promise.all(selectedRowsState.map(s => supplierApi.delete(s.id))); message.success('Suppliers deleted'); actionRef.current?.reload(); setSelectedRows([]); } catch { message.error('Failed to delete'); } }}>Batch delete</Button>
        </div>
      )}
    </PageContainer>
  );
};

export default SupplierList;
