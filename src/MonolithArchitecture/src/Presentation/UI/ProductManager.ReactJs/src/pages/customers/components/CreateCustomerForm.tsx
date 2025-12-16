import { ModalForm, ProFormText } from '@ant-design/pro-components';
import { FormattedMessage } from '@umijs/max';
import { message } from 'antd';
import React from 'react';
import { customerApi } from '@/services/api';

interface Props { visible: boolean; onCancel: () => void; onSuccess: () => void }

const CreateCustomerForm: React.FC<Props> = ({ visible, onCancel, onSuccess }) => {
  const [messageApi, contextHolder] = message.useMessage();
  return (
    <>
      {contextHolder}
      <ModalForm title={<FormattedMessage id="pages.customer.new" defaultMessage="New Customer" />} width={420} visible={visible} onVisibleChange={(v) => !v && onCancel()} onFinish={async (vals) => {
        try { const res = await customerApi.create(vals as any); if (res && res.statusCode === 201) { messageApi.success('Customer created'); onSuccess(); return true; } messageApi.error(res?.message || 'Failed'); return false; } catch { messageApi.error('Failed'); return false; }
      }}>
        <ProFormText name="companyName" label="Company" rules={[{ required: true }]} />
        <ProFormText name="contactName" label="Contact" />
        <ProFormText name="phone" label="Phone" />
        <ProFormText name="country" label="Country" />
      </ModalForm>
    </>
  );
};

export default CreateCustomerForm;
