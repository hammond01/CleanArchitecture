import { ModalForm, ProFormText, ProFormDatePicker } from '@ant-design/pro-components';
import { FormattedMessage } from '@umijs/max';
import { message } from 'antd';
import React from 'react';
import { employeeApi } from '@/services/api';

interface Props { visible: boolean; onCancel: () => void; onSuccess: () => void }

const CreateEmployeeForm: React.FC<Props> = ({ visible, onCancel, onSuccess }) => {
  const [messageApi, contextHolder] = message.useMessage();
  return (
    <>
      {contextHolder}
      <ModalForm title={<FormattedMessage id="pages.employee.new" defaultMessage="New Employee" />} width={480} visible={visible} onVisibleChange={(v) => !v && onCancel()} onFinish={async (vals) => {
        try { const res = await employeeApi.create(vals as any); if (res && res.statusCode === 201) { messageApi.success('Employee created'); onSuccess(); return true; } messageApi.error(res?.message || 'Failed'); return false; } catch { messageApi.error('Failed'); return false; }
      }}>
        <ProFormText name="firstName" label="First Name" rules={[{ required: true }]} />
        <ProFormText name="lastName" label="Last Name" />
        <ProFormText name="title" label="Title" />
        <ProFormText name="city" label="City" />
        <ProFormDatePicker name="hireDate" label="Hire Date" />
      </ModalForm>
    </>
  );
};

export default CreateEmployeeForm;
