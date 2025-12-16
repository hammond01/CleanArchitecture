import type { ActionType, ProColumns } from '@ant-design/pro-components';
import { PageContainer, ProTable } from '@ant-design/pro-components';
import { FormattedMessage } from '@umijs/max';
import { Button, message, Popconfirm } from 'antd';
import React, { useRef, useState } from 'react';
import { employeeApi } from '@/services/api';
import type { Employee } from '@/services/api';
import CreateEmployeeForm from './components/CreateEmployeeForm';
import UpdateEmployeeForm from './components/UpdateEmployeeForm';

const EmployeeList: React.FC = () => {
  const actionRef = useRef<ActionType>(null);
  const [createModalVisible, setCreateModalVisible] = useState<boolean>(false);
  const [updateModalVisible, setUpdateModalVisible] = useState<boolean>(false);
  const [currentRow, setCurrentRow] = useState<Employee>();
  const [selectedRowsState, setSelectedRows] = useState<Employee[]>([]);

  const columns: ProColumns<Employee>[] = [
    { title: 'ID', dataIndex: 'id' },
    { title: 'Name', dataIndex: 'firstName', render: (_, r) => `${r.firstName} ${r.lastName}` },
    { title: 'Title', dataIndex: 'title', responsive: ['md'] },
    { title: 'City', dataIndex: 'city', responsive: ['sm'] },
    { title: 'Hire Date', dataIndex: 'hireDate', responsive: ['lg'] },
    {
      title: 'Actions', dataIndex: 'option', valueType: 'option', render: (_, record) => [
        <a key="edit" onClick={() => { setCurrentRow(record); setUpdateModalVisible(true); }}>Edit</a>,
        <Popconfirm key="delete" title="Are you sure to delete this employee?" onConfirm={async () => { try { await employeeApi.delete(record.id); message.success('Employee deleted'); actionRef.current?.reload(); } catch { message.error('Failed to delete employee'); } }} okText="Yes" cancelText="No"><a key="delete" style={{ color: 'red' }}>Delete</a></Popconfirm>
      ]
    }
  ];

  return (
    <PageContainer>
      <ProTable<Employee>
        headerTitle={<FormattedMessage id="pages.employee.title" defaultMessage="Employee Management" />}
        actionRef={actionRef}
        rowKey="id"
        request={async () => { try { const r = await employeeApi.getAll(); if (r.statusCode === 200) return { data: r.data || [], success: true, total: (r.data || []).length }; return { data: [], success: false, total: 0 }; } catch { return { data: [], success: false, total: 0 }; } }}
        columns={columns}
        toolBarRender={() => [<Button key="new" type="primary" onClick={() => setCreateModalVisible(true)}>New</Button>]}
        rowSelection={{ onChange: (_, rows) => setSelectedRows(rows) }}
      />

      <CreateEmployeeForm visible={createModalVisible} onCancel={() => setCreateModalVisible(false)} onSuccess={() => { setCreateModalVisible(false); actionRef.current?.reload(); }} />

      <UpdateEmployeeForm visible={updateModalVisible} employeeId={currentRow?.id} onCancel={() => setUpdateModalVisible(false)} onSuccess={() => { setUpdateModalVisible(false); setCurrentRow(undefined); actionRef.current?.reload(); }} />

      {selectedRowsState?.length > 0 && (
        <div style={{ marginTop: 16 }}>
          <Button onClick={async () => { try { await Promise.all(selectedRowsState.map(e => employeeApi.delete(e.id))); message.success('Employees deleted'); actionRef.current?.reload(); setSelectedRows([]); } catch { message.error('Failed to delete'); } }}>Batch delete</Button>
        </div>
      )}
    </PageContainer>
  );
};

export default EmployeeList;
